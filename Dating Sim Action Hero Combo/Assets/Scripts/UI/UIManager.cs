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

    // access a specific UI Object by prefab Id
    public UIObject GetUIObject(string uiPrefabId) {
        if(_activeUI.TryGetValue(uiPrefabId, out UIObjectEntry entry)) {
            return entry.UIObject;
        }
        return null;
    }

    // instantiate a new UI object with a given prefab id, onto a specified layer
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
        UIObjectEntry newEntry = new UIObjectEntry(layerId, instancedUIObject);
        _activeUI.Add(uiPrefabId, newEntry);
        return instancedUIObject;
    }

    // destroy a given UI object with a given prefab id
    public void RemoveUIObject(string uiPrefabId) {
        if (!_activeUI.TryGetValue(uiPrefabId, out UIObjectEntry entry)) {
            CustomLogger.Error(nameof(UIManager), $"Could not find UI Prefab with id {uiPrefabId}");
            return;
        }
        Destroy(entry.UIObject.gameObject);
        _activeUI.Remove(uiPrefabId);
    }
}

public class UIObjectEntry {
    public UILayerId LayerId;
    public UIObject UIObject;

    public UIObjectEntry(UILayerId layerId, UIObject uiObject) {
        LayerId = layerId;
        UIObject = uiObject;
    }
}

[System.Serializable]
public class UIPrefabEntry {
    [SerializeField] private string _uiPrefabId;
    [SerializeField] private UILayerId _layerId;

    public string UIPrefabId => _uiPrefabId;
    public UILayerId LayerID => _layerId;
}
