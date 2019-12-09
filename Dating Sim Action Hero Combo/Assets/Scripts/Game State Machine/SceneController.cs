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
    private bool isLoadingScene;

    private void Awake() {
        Instance = this;
    }

    public bool TransitionToScene(string sceneName) {
        /*
        if (CurrentSceneName.Equals(sceneName)) {
            FinishLoadingScene();
            return true;
        }
        */
        if (isLoadingScene) {
            return false;
        }
        _nextSceneName = sceneName;
        // check to see if we're already in that scene or if we're currently loading
        // play some transition animation
        // temp: we're just gonna go straight in
        OnEndSceneTransition();
        return true;
    }

    // when transition animation ends -> should be in loading screen
    private void OnEndSceneTransition() {
        LoadSceneInstant(LoadingScreenScene);
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
        isLoadingScene = false;
        OnSceneLoaded?.Invoke(_nextSceneName);
    }

    public bool LoadSceneInstant(string sceneName) {
        return true;
    }
}
