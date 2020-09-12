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
    private bool _requiresLoadingScreen;
    private bool isLoadingScene;

    private void Awake() {
        Instance = this;
    }

    public bool TransitionToScene(string sceneName, bool requiresLoadingScreen) {
        if (isLoadingScene) {
            CustomLogger.Warn(nameof(SceneController), $"Already loading scene {_nextSceneName}!");
            return false;
        }
        _nextSceneName = sceneName;
        _requiresLoadingScreen = requiresLoadingScreen;
        if (_requiresLoadingScreen) {
            LoadingScreenController.Instance.OnLoadingScreenShowComplete += OnEndSceneTransition;
            LoadingScreenController.Instance.ShowLoadingScreen();
        } else {
            LoadSceneInstant(sceneName);
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
        isLoadingScene = true;
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
        isLoadingScene = false;
        OnSceneLoaded?.Invoke(_nextSceneName);
    }

    private void LoadSceneInstant(string sceneName) {
        SceneManager.LoadScene(sceneName);
        isLoadingScene = false;
        OnSceneLoaded?.Invoke(_nextSceneName);
    }
}
