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
    
    protected override void OnEnter() {
        base.OnEnter();
        StartAnimation();
    }
    
    protected override void OnExit() {
        base.OnExit();
        _unit.AnimationController.OnAnimationStatusUpdated -= OnAnimationStatusUpdated;
    }

    private void OnAnimationStatusUpdated(AnimationStatus status) {
        if(status == AnimationStatus.Completed && _onAnimationCompleteState != null) {
            SetReadyToTransition(_onAnimationCompleteState);
        }
    }
}
