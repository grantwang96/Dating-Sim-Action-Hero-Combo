using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILevelStagingArea : UIObject
{
    [SerializeField] private Text _levelTitle;
    [SerializeField] private Button _startGameBtn;
    [SerializeField] private Button _backBtn;

    [SerializeField] private string _startGameTransitionId;
    [SerializeField] private string _backTransitionId;

    public override bool Initialize() {
        _startGameBtn.onClick.AddListener(OnStartGame);
        _backBtn.onClick.AddListener(OnBack);
        _levelTitle.text = GameLevelDataController.Instance.CurrentGameLevelData.name;
        return base.Initialize();
    }
    public override void CleanUp() {
        _startGameBtn.onClick.RemoveAllListeners();
        _backBtn.onClick.RemoveAllListeners();
        base.CleanUp();
    }

    private void OnStartGame() {
        GameStateManager.Instance.HandleTransition(_startGameTransitionId);
    }

    private void OnBack() {
        GameStateManager.Instance.HandleTransition(_backTransitionId);
    }
}
