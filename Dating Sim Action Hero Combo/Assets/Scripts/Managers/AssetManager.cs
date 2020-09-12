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

    [SerializeField] private List<AssetEntry> _combatPrefabs = new List<AssetEntry>();
    [SerializeField] private List<AssetEntry> _unitPrefabs = new List<AssetEntry>();
    [SerializeField] private List<AssetEntry> _uiPrefabs = new List<AssetEntry>();

    [SerializeField] private Dictionary<string, GameObject> _prefabRegistry = new Dictionary<string, GameObject>();

    private void Awake() {
        if(Instance != null && Instance != this) {
            CustomLogger.Error(nameof(IAssetManager), $" is being set multiple times. Don't do this!");
            return;
        }
        Instance = this;
        LoadPrefabs(_combatPrefabs);
        LoadPrefabs(_unitPrefabs);
        LoadPrefabs(_uiPrefabs);
    }

    private void LoadPrefabs(List<AssetEntry> assetEntries) {
        for(int i = 0; i < assetEntries.Count; i++) {
            _prefabRegistry.Add(assetEntries[i].AssetId, assetEntries[i].Prefab);
        }
    }

    public GameObject GetAsset(string assetName) {
        GameObject go;
        if(!_prefabRegistry.TryGetValue(assetName, out go)) {
            CustomLogger.Error(nameof(AssetManager), $"Could not find object with name {assetName}");
        }
        return go;
    }
}

[System.Serializable]
public class AssetEntry {
    public string AssetId;
    public GameObject Prefab;
}
