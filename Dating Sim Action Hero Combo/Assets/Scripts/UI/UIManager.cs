using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private UILayer[] _layers;

    private readonly Dictionary<UILayerId, UILayer> _uiLayers = new Dictionary<UILayerId, UILayer>();
    private readonly Dictionary<string, UIObjectEntry> _loadedUI = new Dictionary<string, UIObjectEntry>();
    private readonly Dictionary<string, UIObjectEntry> _activeUI = new Dictionary<string, UIObjectEntry>();

    private void Awake() {
        Instance = this;
        InitializeLayers();
    }

    private void InitializeLayers() {
        _uiLayers.Clear();
        for(int i = 0; i < _layers.Length; i++) {
            _uiLayers.Add(_layers[i].UILayerId, _layers[i]);
        }
    }

    public UIObject GetUIObject(string uiPrefabId) {
        if(_activeUI.TryGetValue(uiPrefabId, out UIObjectEntry entry)) {
            return entry.UIObject;
        }
        return null;
    }

    public UIObject CreateNewUIObject(string uiPrefabId, UILayerId layerId) {
        if (_activeUI.ContainsKey(uiPrefabId)) {
            CustomLogger.Error(nameof(UIManager), $"Already contains active UI with prefab Id {uiPrefabId}!");
            return null;
        }
        if (!_uiLayers.TryGetValue(layerId, out UILayer layer)) {
            CustomLogger.Error(nameof(UIManager), $"Layer Id {layerId} not found!");
            return null;
        }
        GameObject obj = AssetManager.Instance.GetAsset(uiPrefabId);
        if(obj == null) {
            CustomLogger.Error(nameof(UIManager), $"Could not retrieve prefab for id {uiPrefabId}!");
            return null;
        }
        UIObject uiObject = obj.GetComponent<UIObject>();
        if (uiObject == null) {
            CustomLogger.Error(nameof(UIManager), $"Prefab {uiPrefabId} did not contain type {nameof(UIObject)}!");
            return null;
        }
        UILayer parent = _uiLayers[layerId];
        UIObject instancedUIObject = Instantiate(uiObject, _uiLayers[layerId].transform);
        UIObjectEntry newEntry = new UIObjectEntry(layerId, uiObject);
        _activeUI.Add(uiPrefabId, newEntry);
        return uiObject;
    }

    public void RemoveUIObject(string uiPrefabId) {
        if (!_activeUI.ContainsKey(uiPrefabId)) {

        }
    }
}

public class UIObjectEntry {
    public readonly UILayerId LayerId;
    public readonly UIObject UIObject;

    public UIObjectEntry(UILayerId layerId, UIObject uiObject) {
        LayerId = layerId;
        UIObject = uiObject;
    }
}
