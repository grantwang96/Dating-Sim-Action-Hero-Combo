using Action = System.Action;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IPooledObjectManager {

    bool RegisterPooledObject(string poolId, int initialCount, Action<bool> OnRegisterComplete = null);
    void DeregisterPooledObject(string poolId);

    bool UsePooledObject(string poolId, out PooledObject obj);
    void ReturnPooledObject(string poolId, PooledObject obj);
}

public class PooledObjectManager : MonoBehaviour, IPooledObjectManager
{
    private const int MaximumObjectPoolSize = 200; // the hard cap maximum a single pool can be

    public static IPooledObjectManager Instance { get; private set; }

    [SerializeField] private PooledObjectLoadEntry[] _objectsToPreload;

    private readonly Dictionary<string, PooledObjectEntry> _objectPool = new Dictionary<string, PooledObjectEntry>();

    private void Awake() {
        if(Instance != null) {
            CustomLogger.Error(this.name, $"There should not be more than 1 {nameof(PooledObjectManager)} instances at a time!");
        }
        Instance = this;
    }

    private void Start() {
        PreloadObjectPool();
    }

    private void PreloadObjectPool() {
        for(int i = 0; i < _objectsToPreload.Length; i++) {
            AssetManager.Instance.GetAsset(_objectsToPreload[i].PoolId);
        }
    }

    public bool RegisterPooledObject(string poolId, int count, Action<bool> OnRegisterComplete = null) {
        PooledObjectEntry entry;
        if (_objectPool.TryGetValue(poolId, out entry)) {
            CloneToPool(poolId, entry.BaseResource, count);
            return true;
        }
        GameObject storedPrefab = AssetManager.Instance.GetAsset(poolId);
        if(storedPrefab == null) {
            CustomLogger.Error(nameof(PooledObjectManager), $"Failed to register object with id {poolId}");
            return false;
        }

        PooledObject pooledObject = storedPrefab.GetComponent<PooledObject>();
        if (pooledObject == null) {
            CustomLogger.Error(nameof(PooledObjectManager), $"Asset is not a pooled object!");
            return false;
        }
        PooledObjectEntry newEntry = new PooledObjectEntry() {
            BaseResource = storedPrefab,
            AvailableObjects = new List<PooledObject>(),
            InUseObjects = new List<PooledObject>()
        };
        _objectPool.Add(poolId, newEntry);
        CloneToPool(poolId, newEntry.BaseResource, count);
        return true;
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
        // despawn all currently spawned pooled objects
        List<PooledObject> objsToDespawn = new List<PooledObject>();
        for(int i = 0; i < _objectPool[objectId].InUseObjects.Count; i++) {
            PooledObject pooledObject = _objectPool[objectId].InUseObjects[i];
            objsToDespawn.Add(pooledObject);
        }
        for(int i = 0; i < objsToDespawn.Count; i++) {
            objsToDespawn[i].Despawn();
        }
        // remove all existing pooled objects
        foreach (PooledObject pooledObject in _objectPool[objectId].AvailableObjects) {
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
}

[System.Serializable]
public class PooledObjectLoadEntry {
    public string PoolId;
    public int InitialCount;
}
