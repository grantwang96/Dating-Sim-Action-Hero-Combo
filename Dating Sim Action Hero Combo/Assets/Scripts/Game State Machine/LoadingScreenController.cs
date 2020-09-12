using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoadingScreenController : IInitializableManager
{
    private const string LoadingScreenPrefabId = "LoadingScreen";

    public static LoadingScreenController Instance { get; private set; }

    public event Action OnLoadingScreenShowComplete;
    public event Action OnLoadingScreenHideComplete;

    private UILoadingScreen _loadingScreen;

    public void Initialize(Action<bool> initializationCallback = null) {
        Instance = this;
        bool success = true;
        UIObject uiObject = UIManager.Instance.CreateNewUIObject(LoadingScreenPrefabId, UILayerId.Overlay);
        _loadingScreen = uiObject as UILoadingScreen;
        if (_loadingScreen == null) {
            CustomLogger.Error(nameof(LoadingScreenController), $"Could not retrieve loading screen prefab with id {LoadingScreenPrefabId}");
            success = false;
        } else {
            _loadingScreen.OnShowComplete += ShowLoadingScreenComplete;
            _loadingScreen.OnHideComplete += HideLoadingScreenComplete;
            _loadingScreen.HideInstant();
        }
        initializationCallback?.Invoke(success);
    }
    
    public void Dispose() {
        _loadingScreen.OnShowComplete -= ShowLoadingScreenComplete;
        _loadingScreen.OnHideComplete -= HideLoadingScreenComplete;
    }

    public void ShowLoadingScreen() {
        _loadingScreen?.Display();
    }

    public void HideLoadingScreen() {
        _loadingScreen?.Hide();
    }
    
    private void ShowLoadingScreenComplete() {
        OnLoadingScreenShowComplete?.Invoke();
    }

    private void HideLoadingScreenComplete() {
        _loadingScreen?.Hide();
        OnLoadingScreenHideComplete?.Invoke();
    }
}
