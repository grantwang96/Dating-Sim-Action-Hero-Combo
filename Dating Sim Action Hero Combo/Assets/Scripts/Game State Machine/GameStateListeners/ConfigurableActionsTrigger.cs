using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurableActionsTrigger : GameStateListener
{
    [SerializeField] private List<ConfigurableGameAction> _gameActions = new List<ConfigurableGameAction>();
    [SerializeField] private string _onActionCompleteTransitionId;

    private int _currentIndex;
    private IConfiguredGameActionState _currentActionState;

    protected override void OnGameStateEntered() {
        _currentIndex = 0;
        PerformNextAction();
    }

    private void PerformNextAction() {
        if(_currentIndex >= _gameActions.Count) {
            OnAllActionsCompleted();
            return;
        }
        ConfigurableGameAction gameAction = _gameActions[_currentIndex];
        _currentIndex++;
        _currentActionState = gameAction.CreateActionState();
        _currentActionState.OnComplete += OnActionCompleted;
        _currentActionState.Execute();
    }

    private void OnActionCompleted() {
        _currentActionState.OnComplete -= OnActionCompleted;
        PerformNextAction();
    }

    private void OnAllActionsCompleted() {
        _currentActionState = null;
        if (!string.IsNullOrEmpty(_onActionCompleteTransitionId)) {
            GameStateManager.Instance.HandleTransition(_onActionCompleteTransitionId);
        }
    }
}
