using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooledObjectManager {
    void RegisterPooledObject(string prefabName, int initialCount);
    void DeregisterPooledObject(string objectId);

    bool UsePooledObject(string objectId, out PooledObject obj);
    void ReturnPooledObject(string objectId, PooledObject obj);
}

public class PooledObjectManager : MonoBehaviour, IPooledObjectManager
{
    private const int MaximumObjectPoolSize = 200; // the hard cap maximum a single pool can be

    public static IPooledObjectManager Instance { get; private set; }

    [SerializeField] private PreloadedPooledObjectEntry[] _objectsToPreload;

    private readonly Dictionary<string, PooledObjectEntry> _objectPool = new Dictionary<string, PooledObjectEntry>();

    private void Awake() {
        if(Instance != null) {
            CustomLogger.Error(this.name, $"There should not be more than 1 {nameof(PooledObjectManager)} instances at a time!");
        }
        Instance = this;
        PreloadObjectPool();
    }

    private void PreloadObjectPool() {
        for(int i = 0; i < _objectsToPreload.Length; i++) {
            GameObject resource = _objectsToPreload[i].Resource;
            PooledObject pooledObject = resource.GetComponent<PooledObject>();
            if(pooledObject == null) {
                CustomLogger.Error(nameof(PooledObjectManager), $"Preload object {resource.name} is not a {nameof(PooledObject)}!");
                continue;
            }
            string pooledObjectId = resource.name;
            PooledObjectEntry newEntry = new PooledObjectEntry() {
                BaseResource = resource,
                AvailableObjects = new List<PooledObject>(),
                InUseObjects = new List<PooledObject>()
            };
            _objectPool.Add(pooledObjectId, newEntry);
        }
    }

    public void RegisterPooledObject(string prefabName, int count) {
        string pooledObjectId = prefabName;

        PooledObjectEntry entry;
        if (!_objectPool.TryGetValue(pooledObjectId, out entry)) {
            // create the file path
            string filePath = GenerateFilePath(prefabName);

            // load the resource
            GameObject resource = Resources.Load<GameObject>(filePath);
            if (resource == null) {
                CustomLogger.Error(nameof(PooledObjectManager), $"Could not retrieve object with path {filePath}!");
                return;
            }
            PooledObject pooledObject = resource.GetComponent<PooledObject>();
            if (pooledObject == null) {
                CustomLogger.Error(nameof(PooledObjectManager), $"Resource was not of type {nameof(PooledObject)}");
                return;
            }

            // add new entry to the pool
            PooledObjectEntry newEntry = new PooledObjectEntry() {
                BaseResource = resource,
                AvailableObjects = new List<PooledObject>(),
                InUseObjects = new List<PooledObject>()
            };
            _objectPool.Add(pooledObjectId, newEntry);
            entry = newEntry;
        }

        CloneToPool(pooledObjectId, entry.BaseResource, count);
    }
    
    private string GenerateFilePath(string prefabName) {
        try {
            string[] components = prefabName.Split('.');
            string[] pathComponents = components[1].Split('_');
            string filePath = pathComponents[0];
            for (int i = 1; i < pathComponents.Length - 1; i++) {
                filePath = $"{filePath}/{pathComponents[i]}";
            }
            filePath = $"{filePath}/{prefabName}";
            return filePath;
        } catch (Exception e) {
            CustomLogger.Error(nameof(PooledObjectManager), $"Error parsing file path {e.StackTrace}");
            return string.Empty;
        }
    }

    private void CloneToPool(string poolId, GameObject resource, int count) {
        PooledObjectEntry entry = _objectPool[poolId];
        int currentCount = entry.AvailableObjects.Count + entry.InUseObjects.Count;
        if(currentCount + count > MaximumObjectPoolSize) {
            CustomLogger.Warn(nameof(PooledObjectManager), $"Max pool size reached for {poolId}");
            count = MaximumObjectPoolSize - currentCount;
        }
        for (int i = 0; i < count; i++) {
            GameObject clone = Instantiate(resource, transform);
            clone.name = poolId;
            PooledObject clonePO = clone.GetComponent<PooledObject>();
            _objectPool[poolId].AvailableObjects.Add(clonePO);
            clonePO.Despawn(); // hide the object
        }
    }

    public void DeregisterPooledObject(string objectId) {
        if (!_objectPool.ContainsKey(objectId)) {
            CustomLogger.Error(nameof(PooledObjectManager), $"Does not contain entry with id {objectId}!");
            return;
        }
        foreach(PooledObject pooledObject in _objectPool[objectId].AvailableObjects) {
            UnityEngine.Object obj = pooledObject as UnityEngine.Object;
            Destroy(obj);
        }
        foreach(PooledObject pooledObject in _objectPool[objectId].InUseObjects) {
            UnityEngine.Object obj = pooledObject as UnityEngine.Object;
            Destroy(obj);
        }
        _objectPool.Remove(objectId);
    }

    public bool UsePooledObject(string objectId, out PooledObject obj) {
        obj = null;
        bool success = false;
        PooledObjectEntry entry;
        if(_objectPool.TryGetValue(objectId, out entry)) {
            if(entry.AvailableObjects.Count == 0) {
                CloneToPool(objectId, entry.BaseResource, 1);
            }
            obj = entry.AvailableObjects[0];
            entry.InUseObjects.Add(obj);
            entry.AvailableObjects.RemoveAt(0);
            success = true;
        }
        return success;
    }

    public void ReturnPooledObject(string objectId, PooledObject obj) {
        PooledObjectEntry entry;
        if(!_objectPool.TryGetValue(objectId, out entry)) {
            CustomLogger.Error(nameof(PooledObjectManager), $"Could not find in-use object pool for id: {objectId}");
            return;
        }
        entry.InUseObjects.Remove(obj);
        if (!_objectPool[objectId].AvailableObjects.Contains(obj)) {
            _objectPool[objectId].AvailableObjects.Add(obj);
        }
    }

    private class PooledObjectEntry {
        public GameObject BaseResource;
        public List<PooledObject> AvailableObjects = new List<PooledObject>();
        public List<PooledObject> InUseObjects = new List<PooledObject>();
    }

    [Serializable]
    private class PreloadedPooledObjectEntry {
        public GameObject Resource;
        public int InitialCount;
    }
}
