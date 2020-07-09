using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Chase : AIState
{
    [SerializeField] private bool _fullSpeed;
    [SerializeField] private NPCTargetManager _targetManager;
    [SerializeField] private NPCNavigator _navigator;
    [SerializeField] private NPCMoveController _moveController;

    [SerializeField] private AIState _onArrivedDestination;
    [SerializeField] private AIState _onFailedToPath;

    public override void Enter(AIStateInitializationData initData = null) {
        base.Enter(initData);
        PathStatus status = _navigator.SetDestination(_moveController.MapPosition, _targetManager.CurrentTarget.MoveController.MapPosition);
        if(status == PathStatus.Invalid) {
            SetReadyToTransition(_onFailedToPath);
            return;
        }
        _navigator.LookTarget = null;
        float speed = _fullSpeed ? _unit.UnitData.RunSpeed : _unit.UnitData.WalkSpeed;
        _moveController.SetSpeed(speed);
        _navigator.OnArrivedFinalDestination += OnArrivedFinalDestination;
        _navigator.SetDestination(_moveController.MapPosition, _navigator.TargetPosition);
    }

    public override void Exit(AIState nextState) {
        base.Exit(nextState);
        _navigator.ClearDestination();
    }

    private void OnArrivedFinalDestination() {
        _navigator.OnArrivedFinalDestination -= OnArrivedFinalDestination;
        SetReadyToTransition(_onArrivedDestination);
    }
}
