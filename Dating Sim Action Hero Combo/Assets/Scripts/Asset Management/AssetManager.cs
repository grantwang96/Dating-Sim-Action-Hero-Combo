using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: DON'T DO ASSET BUNDLES. GO WITH DIRECT REFERENCES. BUNDLING DOESN'T SEEM TO BE WORTH IT AT THIS TIME +12/6/2019

public interface IAssetManager {
    GameObject GetAsset(string assetName);
}

public class AssetManager : MonoBehaviour, IAssetManager {
    
    public static IAssetManager Instance { get; private set; }

    private Dictionary<string, AssetBundleRequest> _assetBundleRequest = new Dictionary<string, AssetBundleRequest>();

    [SerializeField] private List<GameObject> _allPrefabs = new List<GameObject>();

    [SerializeField] private Dictionary<string, GameObject> _pr1efabRegistry = new Dictionary<string, GameObject>();

    private void Awake() {
        if(Instance != null && Instance != this) {
            CustomLogger.Error(nameof(IAssetManager), $" is being set multiple times. Don't do this!");
            return;
        }
        Instance = this;
        LoadPooledObjects();
    }

    private void LoadPooledObjects() {
        for(int i = 0; i < _allPrefabs.Count; i++) {
            _pr1efabRegistry.Add(_allPrefabs[i].name, _allPrefabs[i]);
        }
    }

    public GameObject GetAsset(string assetName) {
        GameObject go;
        if(!_pr1efabRegistry.TryGetValue(assetName, out go)) {
            CustomLogger.Error(nameof(AssetManager), $"Could not find object with name {assetName}");
        }
        return go;
    }
}
