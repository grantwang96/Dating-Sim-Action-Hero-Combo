using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : UIObject
{
    private const string ResumeGameTransitionId = "resume_game";
    private const string QuitGameTransitionId = "quit_game";

    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _quitButton;

    public override bool Initialize() {
        _resumeButton.onClick.AddListener(OnResumeBtnPressed);
        _quitButton.onClick.AddListener(OnQuitBtnPressed);
        return base.Initialize();
    }

    public override void Display() {
        base.Display();
        gameObject.SetActive(true);
    }

    public override void Hide() {
        base.Hide();
        gameObject.SetActive(false);
    }

    public override void CleanUp() {
        base.CleanUp();
        _resumeButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();
    }

    private void OnResumeBtnPressed() {
        GameStateManager.Instance.HandleTransition(ResumeGameTransitionId);
    }

    private void OnQuitBtnPressed() {
        GameStateManager.Instance.HandleTransition(QuitGameTransitionId);
    }
}
