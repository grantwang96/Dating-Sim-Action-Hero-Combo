using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurableActionsTrigger : GameStateListener
{
    [SerializeField] private List<ConfigurableGameAction> _onStateEnterGameActions = new List<ConfigurableGameAction>();
    [SerializeField] private List<ConfigurableGameAction> _onStateExitGameActions = new List<ConfigurableGameAction>();
    [SerializeField] private string _onStateEnterActionsCompleteTransitionId;

    private int _currentIndex;
    private IConfiguredGameActionState _currentActionState;

    protected override void OnGameStateEntered() {
        _currentIndex = 0;
        PerformNextStateEnterAction();
    }

    protected override void OnGameStateExited() {
        _currentIndex = 0;
        PerformNextStateExitAction();
        base.OnGameStateExited();
    }

    private void PerformNextStateEnterAction() {
        if(_currentIndex >= _onStateEnterGameActions.Count) {
            OnAllActionsCompleted(_onStateEnterActionsCompleteTransitionId);
            return;
        }
        ConfigurableGameAction gameAction = _onStateEnterGameActions[_currentIndex];
        _currentIndex++;
        _currentActionState = gameAction.CreateActionState();
        _currentActionState.OnComplete += OnStateEnterActionCompleted;
        _currentActionState.Execute();
    }

    private void OnStateEnterActionCompleted() {
        _currentActionState.OnComplete -= OnStateEnterActionCompleted;
        PerformNextStateEnterAction();
    }

    private void PerformNextStateExitAction() {
        if (_currentIndex >= _onStateExitGameActions.Count) {
            OnAllActionsCompleted();
            return;
        }
        ConfigurableGameAction gameAction = _onStateExitGameActions[_currentIndex];
        _currentIndex++;
        _currentActionState = gameAction.CreateActionState();
        _currentActionState.OnComplete += OnStateExitActionCompleted;
        _currentActionState.Execute();
    }

    private void OnStateExitActionCompleted() {
        _currentActionState.OnComplete -= OnStateExitActionCompleted;
        PerformNextStateExitAction();
    }

    private void OnAllActionsCompleted(string transitionId = null) {
        _currentActionState = null;
        if (!string.IsNullOrEmpty(transitionId)) {
            GameStateManager.Instance.HandleTransition(transitionId);
        }
    }
}
