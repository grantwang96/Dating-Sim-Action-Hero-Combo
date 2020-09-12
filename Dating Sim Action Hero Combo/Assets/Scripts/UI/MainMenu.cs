using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string _playBtnTransitionName;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;

    private void Awake() {
        _playButton.onClick.AddListener(OnPlayBtnPressed);
        _quitButton.onClick.AddListener(OnQuitBtnPressed);
    }

    private void OnDestroy() {
        _playButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();
    }

    private void OnPlayBtnPressed() {
        GameStateManager.Instance.HandleTransition(_playBtnTransitionName);
    }

    private void OnQuitBtnPressed() {
        Application.Quit();
    }
}
