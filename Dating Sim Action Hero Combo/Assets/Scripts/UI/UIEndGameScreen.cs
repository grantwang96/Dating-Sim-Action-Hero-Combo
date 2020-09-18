using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEndGameScreen : UIObject
{
    [SerializeField] private Text _resultMessage;
    [SerializeField] private Button _replayBtn;
    [SerializeField] private Button _returnToMainMenuBtn;

    public override bool Initialize() {
        _replayBtn.onClick.AddListener(OnReplay);
        _returnToMainMenuBtn.onClick.AddListener(OnReturnToMainMenu);
        return base.Initialize();
    }

    public override void Display() {
        base.Display();
        gameObject.SetActive(true);
    }

    private void SetInfo() {
        _resultMessage.text = GameManager.Instance.EndGameContext?.WonGame ?? false ? "Victory!" : "Defeat!";
    }

    public override void Hide() {
        base.Hide();
        gameObject.SetActive(false);
    }

    public override void CleanUp() {
        base.CleanUp();
        _replayBtn.onClick.RemoveAllListeners();
        _returnToMainMenuBtn.onClick.RemoveAllListeners();
    }

    // todo
    private void OnReplay() {
        
    }

    private void OnReturnToMainMenu() {
        GameManager.Instance.ExitGame();
    }
}
