using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public delegate void SceneUpdateDelegate(string sceneName);

public class SceneController : MonoBehaviour {

    private const string LoadingScreenScene = "LoadingScreen";

	public static SceneController Instance { get; private set; }

    public string CurrentSceneName => SceneManager.GetActiveScene().name;

    public event SceneUpdateDelegate OnSceneLoaded;

    private string _nextSceneName;
    private bool _isLoadingScene;

    private void Awake() {
        Instance = this;
    }

    public bool TransitionToScene(string sceneName, bool requiresLoadingScreen) {
        Debug.Log($"Loading scene {sceneName}...");
        if (_isLoadingScene) {
            CustomLogger.Warn(nameof(SceneController), $"Already loading scene {_nextSceneName}!");
            return false;
        }
        if(CurrentSceneName == sceneName) {
            CustomLogger.Log(nameof(SceneController), $"Already in scene {CurrentSceneName}!");
            return true;
        }
        _isLoadingScene = true;
        _nextSceneName = sceneName;
        if (requiresLoadingScreen) {
            LoadingScreenController.Instance.OnLoadingScreenShowComplete += OnEndSceneTransition;
            LoadingScreenController.Instance.ShowLoadingScreen();
        } else {
            StartCoroutine(LoadSceneOneFrame());
        }
        return true;
    }

    // when transition animation ends -> should be in loading screen
    private void OnEndSceneTransition() {
        LoadingScreenController.Instance.OnLoadingScreenShowComplete -= OnEndSceneTransition;
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync() {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_nextSceneName);
        while (!asyncLoad.isDone) {
            yield return new WaitForEndOfFrame();
        }
        FinishLoadingScene();
    }

    private void FinishLoadingScene() {
        LoadingScreenController.Instance.OnLoadingScreenHideComplete += OnLoadingScreenHideComplete;
        LoadingScreenController.Instance.HideLoadingScreen();
    }

    private void OnLoadingScreenHideComplete() {
        LoadingScreenController.Instance.OnLoadingScreenHideComplete -= OnLoadingScreenHideComplete;
        _isLoadingScene = false;
        // Debug.Log("Scene load complete!");
        OnSceneLoaded?.Invoke(_nextSceneName);
    }

    private void LoadSceneInstant(string sceneName) {
        _isLoadingScene = false;
        SceneManager.LoadScene(sceneName);
        // Debug.Log("Scene load complete!");
        OnSceneLoaded?.Invoke(_nextSceneName);
    }

    private IEnumerator LoadSceneOneFrame() {
        _isLoadingScene = true;
        yield return new WaitForEndOfFrame();
        LoadSceneInstant(_nextSceneName);
    }
}
