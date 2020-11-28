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
    
    protected override void OnEnter() {
        base.OnEnter();
        // Get path to next destination here
        if (_moveController == null) {
            return;
        }
        GetNextWanderPosition();
    }

    private void GetNextWanderPosition() {
        List<IntVector3> positions = MapService.GetTraversableTiles(
            _wanderRangeMax,
            _moveController.MapPosition,
            _unit,
            _unit.UnitData.TraversableThreshold,
            _wanderRangeMin);
        _unit.Navigator.OnArrivedFinalDestination += OnArrivedFinalDestination;
        if (positions.Count == 0) {
            CustomLogger.Warn(nameof(AIState_Wander), $"Could not find available tile to traverse to!");
            OnArrivedFinalDestination();
            return;
        }
        _unit.Navigator.SetDestination(_moveController.MapPosition, positions[Random.Range(0, positions.Count)]);
        float speed = _fullSpeed ? _unit.UnitData.RunSpeed : _unit.UnitData.WalkSpeed;
        _moveController.SetSpeed(speed);
    }
    
    protected override void OnExit() {
        _unit.Navigator.ClearDestination();
        base.OnExit();
    }

    private void OnArrivedFinalDestination() {
        _unit.Navigator.OnArrivedFinalDestination -= OnArrivedFinalDestination;
        SetReadyToTransition(_onArrivedDestination);
    }
}
