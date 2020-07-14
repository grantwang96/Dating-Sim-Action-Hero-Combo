using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Wander : AIState
{
    [SerializeField] private bool _fullSpeed;
    [SerializeField] private NPCMoveController _moveController;
    [SerializeField] private int _wanderRangeMin;
    [SerializeField] private int _wanderRangeMax;

    [SerializeField] private AIState _onArrivedDestination;

    public override void Enter(AIStateInitializationData initData = null) {
        // Get path to next destination here
        if (_moveController == null) {
            return;
        }
        GetNextWanderPosition();
        base.Enter(initData);
    }

    private void GetNextWanderPosition() {
        List<IntVector3> positions = MapService.GetPositionsWithinRadius(_wanderRangeMin, _moveController.MapPosition, _wanderRangeMax);
        if(positions.Count == 0) {
            OnArrivedFinalDestination();
            return;
        }
        _unit.Navigator.SetDestination(_moveController.MapPosition, positions[Random.Range(0, positions.Count)]);
        _unit.Navigator.OnArrivedFinalDestination += OnArrivedFinalDestination;
        float speed = _fullSpeed ? _unit.UnitData.RunSpeed : _unit.UnitData.WalkSpeed;
        _moveController.SetSpeed(speed);
    }

    public override void Exit(AIState nextState) {
        _unit.Navigator.ClearDestination();
        base.Exit(nextState);
    }

    private void OnArrivedFinalDestination() {
        _unit.Navigator.OnArrivedFinalDestination -= OnArrivedFinalDestination;
        SetReadyToTransition(_onArrivedDestination);
    }
}
