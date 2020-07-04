using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_SingleAnimation : AIState {

    private const string AlertStateName = "Alert";
    
    [SerializeField] private AnimationStateData _animationData;
    [SerializeField] private AIState _onAnimationCompleteState;
    
    private void StartAnimation() {
        _unit.AnimationController.UpdateState(_animationData);
        _unit.AnimationController.OnAnimationStatusUpdated += OnAnimationStatusUpdated;
    }

    public override void Enter(AIStateInitializationData initData = null) {
        base.Enter(initData);
        StartAnimation();
    }

    public override void Exit(AIState nextState) {
        base.Exit(nextState);
        _unit.AnimationController.OnAnimationStatusUpdated -= OnAnimationStatusUpdated;
    }

    private void OnAnimationStatusUpdated(AnimationStatus status) {
        if(status == AnimationStatus.Completed) {
            SetReadyToTransition(_onAnimationCompleteState);
        }
    }
}
