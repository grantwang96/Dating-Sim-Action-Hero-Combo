using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : UIObject
{
    [SerializeField] private string _playBtnTransitionName;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _quitButton;

    public override bool Initialize() {
        _playButton.onClick.AddListener(OnPlayBtnPressed);
        _quitButton.onClick.AddListener(OnQuitBtnPressed);
        return base.Initialize();
    }

    public override void CleanUp() {
        _playButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();
        base.CleanUp();
    }
    private void OnPlayBtnPressed() {
        GameStateManager.Instance.HandleTransition(_playBtnTransitionName);
    }

    private void OnQuitBtnPressed() {
        Application.Quit();
    }
}
