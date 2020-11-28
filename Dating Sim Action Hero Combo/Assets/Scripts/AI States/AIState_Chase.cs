using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Chase : AIState
{
    [SerializeField] private bool _fullSpeed;
    [SerializeField] private NPCMoveController _moveController;
    [SerializeField] private AIState _onLostTarget;
    [SerializeField] private AIState _onFailedToPath;
    [SerializeField] private float _maxChaseDuration;

    private float _currentChaseDuration;
    
    protected override void OnEnter() {
        base.OnEnter();
        _currentChaseDuration = 0f;
        SetPathToTarget();
    }

    private void SetPathToTarget() {
        IntVector3 destination = GetClosestAvailableTile();
        if(destination == _unit.MoveController.MapPosition) {
            CustomLogger.Warn(nameof(AIState_Chase), $"Could not find closest available tile!");
            _unit.Navigator.OnArrivedFinalDestination -= OnArrivedFinalDestination;
            SetReadyToTransition(_onFailedToPath);
        }
        PathStatus status = _unit.Navigator.SetDestination(
            _unit.MoveController.MapPosition,
            destination);
        if (status == PathStatus.Invalid) {
            CustomLogger.Warn(nameof(AIState_Chase), $"Could not path to destination: {destination}!");
            _unit.Navigator.OnArrivedFinalDestination -= OnArrivedFinalDestination;
            SetReadyToTransition(_onFailedToPath);
            return;
        }
        _unit.Navigator.OnArrivedFinalDestination += OnArrivedFinalDestination;
        _unit.Navigator.LookTarget = null;
        float speed = _fullSpeed ? _unit.UnitData.RunSpeed : _unit.UnitData.WalkSpeed;
        _moveController.SetSpeed(speed);
    }

    private IntVector3 GetClosestAvailableTile() {
        List<IntVector3> positions = MapService.GetTraversableTiles
            (1, _unit.TargetManager.CurrentTarget.MoveController.MapPosition, _unit, _unit.UnitData.TraversableThreshold, 0);
        if(positions.Count == 0) {
            CustomLogger.Warn(nameof(AIState_Chase), $"Could not find any positions to path to!");
            return _unit.MoveController.MapPosition;
        }
        return positions[Random.Range(0, positions.Count)];
    }

    public override void Execute() {
        base.Execute();
        _currentChaseDuration += Time.deltaTime;
        if(_currentChaseDuration >= _maxChaseDuration) {
            _unit.Navigator.OnArrivedFinalDestination -= OnArrivedFinalDestination;
            SetReadyToTransition(_onLostTarget);
        }
    }

    public override void Exit(AIState nextState) {
        base.Exit(nextState);
        _unit.Navigator.ClearDestination();
    }

    private void OnArrivedFinalDestination() {
        _unit.Navigator.OnArrivedFinalDestination -= OnArrivedFinalDestination;
        SetPathToTarget();
    }
}
