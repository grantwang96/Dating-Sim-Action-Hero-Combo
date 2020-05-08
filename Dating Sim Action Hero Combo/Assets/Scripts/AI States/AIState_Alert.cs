using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Alert : AIState {

    private const string AlertStateName = "Alert";

    [SerializeField] private NPCMoveController _moveController;
    [SerializeField] private Animator _animator;

    private bool _isComplete;
    
    private void StartAnimation() {
        AnimatorClipInfo[] clipInfos = _animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfos.Length == 0) {
            return;
        }
        _animator.Play(AlertStateName);
    }

    public override void Enter(AIStateInitializationData initData = null) {
        base.Enter(initData);
        _isComplete = false;
        _moveController.ClearDestination();
        StartAnimation();
    }

    public override bool Execute() {
        base.Execute();
        if (_isComplete) {
            return true;
        }
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        if (info.IsName(AIState_Alert.AlertStateName) && info.normalizedTime >= 1f) {
            OnAlertAnimationComplete();
            return true;
        }
        return false;
    }

    private void OnAlertAnimationComplete() {
        // change animation state
        // temp: Force change animation to normal
        _animator.Play("Normal");
        SetNextTransition(AIStateTransitionId.OnUnitAlerted);
        _isComplete = true;
    }
}
