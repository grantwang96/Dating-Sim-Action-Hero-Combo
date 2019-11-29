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
    private const int MaximumObjectPoolSize = 100; // the hard cap maximum a single pool can be

    public static IPooledObjectManager Instance { get; private set; }

    private readonly Dictionary<string, PooledObjectEntry> _availablePooledObjects = new Dictionary<string, PooledObjectEntry>();
    private readonly Dictionary<string, List<PooledObject>> _inUsePooledObjects = new Dictionary<string, List<PooledObject>>();

    private void Awake() {
        if(Instance != null) {
            CustomLogger.Error(this.name, $"There should not be more than 1 {nameof(PooledObjectManager)} instances at a time!");
        }
        Instance = this;
    }

    public void RegisterPooledObject(string prefabName, int count) {
        string pooledObjectId = prefabName;

        PooledObjectEntry entry;
        if (!_availablePooledObjects.TryGetValue(pooledObjectId, out entry)) {
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

            // add new entry to the available pool
            PooledObjectEntry newEntry = new PooledObjectEntry() {
                BaseResource = resource,
                AvailableObjects = new List<PooledObject>()
            };
            _availablePooledObjects.Add(pooledObjectId, newEntry);
            _inUsePooledObjects.Add(pooledObjectId, new List<PooledObject>());
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
        for (int i = 0; i < count; i++) {
            GameObject clone = Instantiate(resource, transform);
            clone.name = poolId;
            PooledObject clonePO = clone.GetComponent<PooledObject>();
            _availablePooledObjects[poolId].AvailableObjects.Add(clonePO);
            clonePO.Despawn(); // hide the object
        }
    }

    public void DeregisterPooledObject(string objectId) {
        if (!_availablePooledObjects.ContainsKey(objectId)) {
            CustomLogger.Error(nameof(PooledObjectManager), $"Does not contain entry with id {objectId}!");
            return;
        }
        foreach(PooledObject pooledObject in _availablePooledObjects[objectId].AvailableObjects) {
            UnityEngine.Object obj = pooledObject as UnityEngine.Object;
            Destroy(obj);
        }
        _availablePooledObjects.Remove(objectId);
        foreach(PooledObject pooledObject in _inUsePooledObjects[objectId]) {
            UnityEngine.Object obj = pooledObject as UnityEngine.Object;
            Destroy(obj);
        }
        _inUsePooledObjects.Remove(objectId);
    }

    public bool UsePooledObject(string objectId, out PooledObject obj) {
        obj = null;
        bool success = false;
        PooledObjectEntry entry;
        if(_availablePooledObjects.TryGetValue(objectId, out entry)) {
            if(entry.AvailableObjects.Count == 0) {
                CloneToPool(objectId, entry.BaseResource, 1);
            }
            obj = entry.AvailableObjects[0];
            _inUsePooledObjects[objectId].Add(obj);
            entry.AvailableObjects.RemoveAt(0);
            success = true;
        }
        return success;
    }

    public void ReturnPooledObject(string objectId, PooledObject obj) {
        List<PooledObject> inUsePool;
        if(!_inUsePooledObjects.TryGetValue(objectId, out inUsePool)) {
            CustomLogger.Error(nameof(PooledObjectManager), $"Could not find in-use object pool for id: {objectId}");
            return;
        }
        inUsePool.Remove(obj);
        _availablePooledObjects[objectId].AvailableObjects.Add(obj);
    }

    private class PooledObjectEntry {
        public GameObject BaseResource;
        public List<PooledObject> AvailableObjects = new List<PooledObject>();
    }
}
