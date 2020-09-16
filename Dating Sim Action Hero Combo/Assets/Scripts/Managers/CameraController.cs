using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    public Camera MainCamera { get; private set; }

    private void Awake() {
        Instance = this;
        MainCamera = Camera.main;
    }

    private void Start() {
        SceneController.Instance.OnSceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy() {
        SceneController.Instance.OnSceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(string sceneName) {
        MainCamera = Camera.main;
    }
}
