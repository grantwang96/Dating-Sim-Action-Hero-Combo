using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEndGameScreen : UIObject
{
    [SerializeField] private string _replayGameTransitionId;
    [SerializeField] private string _mainMenuTransitiionId;
    [SerializeField] private Text _resultHeader;
    [SerializeField] private Text _resultDescription;
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
        SetInfo();
    }

    private void SetInfo() {
        EndGameContext context = GameManager.Instance.EndGameContext;
        if(context == null) {
            Debug.LogError($"[{nameof(UIEndGameScreen)}]: No end game context was set!");
            return;
        }
        _resultHeader.text = context?.WonGame ?? false ? "Victory!" : "Defeat!";
        switch (context.EndResult) {
            case EndResult.AllQuestsCompleted:
                _resultDescription.text = "Mission accomplished!";
                break;
            case EndResult.PlayerDefeated:
                _resultDescription.text = "You were eliminated!";
                break;
            case EndResult.DateDefeated:
                _resultDescription.text = "Date was eliminated!";
                break;
            case EndResult.DateTurnedAway:
                _resultDescription.text = "Date was unhappy!";
                break;
            case EndResult.IdentityDiscovered:
                _resultDescription.text = "You've been discovered!";
                break;
            default:
                _resultDescription.text = "Invalid description!";
                break;
        }
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

    private void OnReplay() {
        GameStateManager.Instance.HandleTransition(_replayGameTransitionId);
    }

    private void OnReturnToMainMenu() {
        GameStateManager.Instance.HandleTransition(_mainMenuTransitiionId);
    }
}
