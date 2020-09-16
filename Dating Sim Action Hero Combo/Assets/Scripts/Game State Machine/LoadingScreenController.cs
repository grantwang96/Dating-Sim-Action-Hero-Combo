using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoadingScreenController : IInitializableManager
{
    private const string LoadingScreenPrefabId = "LoadingScreen";

    public static LoadingScreenController Instance => GetOrSetInstance();
    private static LoadingScreenController _instance;

    public event Action OnLoadingScreenShowComplete;
    public event Action OnLoadingScreenHideComplete;

    private UILoadingScreen _loadingScreen;

    private static LoadingScreenController GetOrSetInstance() {
        if(_instance == null) {
            _instance = new LoadingScreenController();
        }
        return _instance;
    }

    public void Initialize(Action<bool> initializationCallback = null) {
        bool success = true;
        if(_loadingScreen == null) {
            UIObject uiObject = UIManager.Instance.CreateNewUIObject(LoadingScreenPrefabId, UILayerId.Overlay);
            _loadingScreen = uiObject as UILoadingScreen;
            if (_loadingScreen == null) {
                CustomLogger.Error(nameof(LoadingScreenController), $"Could not retrieve loading screen prefab with id {LoadingScreenPrefabId}");
                success = false;
            } else {
                Debug.Log("Loading screen initialized!");
                _loadingScreen.OnShowComplete += ShowLoadingScreenComplete;
                _loadingScreen.OnHideComplete += HideLoadingScreenComplete;
                _loadingScreen.HideInstant();
            }
        }
        Debug.Log("Initialized loading screen controller!");
        initializationCallback?.Invoke(success);
    }
    
    public void Dispose() {
        _loadingScreen.OnShowComplete -= ShowLoadingScreenComplete;
        _loadingScreen.OnHideComplete -= HideLoadingScreenComplete;
    }

    public void ShowLoadingScreen() {
        Debug.Log("Show loading screen");
        _loadingScreen?.Display();
    }

    public void HideLoadingScreen() {
        Debug.Log("Hide loading screen");
        _loadingScreen?.Hide();
    }
    
    private void ShowLoadingScreenComplete() {
        OnLoadingScreenShowComplete?.Invoke();
    }

    private void HideLoadingScreenComplete() {
        OnLoadingScreenHideComplete?.Invoke();
    }
}
