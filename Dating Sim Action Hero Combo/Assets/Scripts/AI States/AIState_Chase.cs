using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Chase : AIState
{
    [SerializeField] private bool _fullSpeed;
    [SerializeField] private NPCMoveController _moveController;
    [SerializeField] private AIState _onArrivedDestination;
    [SerializeField] private AIState _onFailedToPath;

    public override void Enter(AIStateInitializationData initData = null) {
        base.Enter(initData);
        PathStatus status = _unit.Navigator.SetDestination(_unit.MoveController.MapPosition, _unit.TargetManager.CurrentTarget.MoveController.MapPosition);
        if(status == PathStatus.Invalid) {
            SetReadyToTransition(_onFailedToPath);
            return;
        }
        _unit.Navigator.LookTarget = null;
        float speed = _fullSpeed ? _unit.UnitData.RunSpeed : _unit.UnitData.WalkSpeed;
        _moveController.SetSpeed(speed);
        _unit.Navigator.OnArrivedFinalDestination += OnArrivedFinalDestination;
        _unit.Navigator.SetDestination(_moveController.MapPosition, _unit.Navigator.TargetPosition);
    }

    public override void Exit(AIState nextState) {
        base.Exit(nextState);
        _unit.Navigator.ClearDestination();
    }

    private void OnArrivedFinalDestination() {
        _unit.Navigator.OnArrivedFinalDestination -= OnArrivedFinalDestination;
        SetReadyToTransition(_onArrivedDestination);
    }
}
