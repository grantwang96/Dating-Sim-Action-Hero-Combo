using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Alert")]
public class AIState_Alert : AIStateDataObject {

    public const string AlertStateName = "Alert";

    protected override ActiveAIState GenerateActiveAIState(NPCUnitController controller) {
        ActiveAlertState activeAlertState = new ActiveAlertState(controller.Unit);
        return activeAlertState;
    }
}

public class ActiveAlertState : ActiveAIState {

    private Animator _animator;

    private bool _isComplete;

    public ActiveAlertState(Unit unit) {
        _animator = unit.Animator;
        _isComplete = false;
        StartAnimation();
        NPCMoveController moveController = unit.GetComponent<NPCMoveController>();
        if(moveController != null) {
            moveController.ClearDestination();
        }
    }

    private void StartAnimation() {
        AnimatorClipInfo[] clipInfos = _animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfos.Length == 0) {
            return;
        }
        _animator.Play(AIState_Alert.AlertStateName);
    }

    public override bool OnExecute() {
        base.OnExecute();
        if (_isComplete) {
            return true;
        }
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        if(info.IsName(AIState_Alert.AlertStateName) && info.normalizedTime >= 1f) {
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
