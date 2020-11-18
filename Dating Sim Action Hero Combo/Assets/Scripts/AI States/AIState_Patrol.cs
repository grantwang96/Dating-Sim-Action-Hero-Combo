using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Patrol : AIState
{
    [SerializeField] private AIState _onFailedFindPatrolLoop;
    [SerializeField] private AIState _onFailedToPath;
    [Tooltip("This should be a child state")]
    [SerializeField] private AIState _onArrivedAtPatrolPoint;
    [Tooltip("This should be a child state")]
    [SerializeField] private AIState _onContinuePatrol;
    [SerializeField] private NPCMoveController _moveController;

    private PatrolLoop _patrolLoop;
    private int _currentPatrolLoopIndex;
    private bool _isHolding;
    [SerializeField] private float _totalHoldTime;
    private float _currentHoldTime;

    protected override void OnEnter() {
        base.OnEnter();
        // get the relevant enemy info
        if (!EnemyManager.Instance.TryGetEnemyInfo(_unit.UnitId, out IEnemyInfo enemyInfo)) {
            Debug.LogError($"[{_unit.UnitId}/{nameof(AIState_Patrol)}]: Failed to get enemy info from unit id {_unit.UnitId}!");
            SetReadyToTransition(_onFailedFindPatrolLoop);
            return;
        }
        // get the patrol loop using the id in enemy info
        if (!LevelDataManager.Instance.TryGetPatrolLoop(enemyInfo.PatrolLoopId, out _patrolLoop)) {
            Debug.LogError($"[{_unit.UnitId}/{nameof(AIState_Patrol)}]: Failed to get patrol loop from id {enemyInfo.PatrolLoopId}!");
            SetReadyToTransition(_onFailedFindPatrolLoop);
            return;
        }
        // get the nearest path node
        _currentPatrolLoopIndex = GetNearestPatrolPointIndex();
        _currentHoldTime = 0f;
        _isHolding = false;
        // listen to arrival events
        _unit.Navigator.OnArrivedFinalDestination += OnArrivedDestination;
        _moveController.SetSpeed(_unit.UnitData.WalkSpeed);
        SetNextDestination();
    }

    public override void Execute() {
        base.Execute();
        if (_isHolding) {
            _currentHoldTime += Time.deltaTime;
            if(_currentHoldTime >= _totalHoldTime) {
                _isHolding = false;
                SetNextDestination();
                _moveController.SetSpeed(_unit.UnitData.WalkSpeed);
                SetReadyToTransition(_onContinuePatrol);
            }
        }
    }

    protected override void OnExit() {
        base.OnExit();
        _unit.Navigator.OnArrivedFinalDestination -= OnArrivedDestination;
    }

    private int GetNearestPatrolPointIndex() {
        // get current position
        int startX = _unit.MoveController.MapPosition.x;
        int startY = _unit.MoveController.MapPosition.y;
        int closestPointIndex = 0;
        IntVector3 patrolPointMapPosition = LevelDataManager.Instance.WorldToArraySpace(_patrolLoop.PatrolPoints[closestPointIndex].position);
        int closestDistance = MapService.DistanceFromStart(startX, startY, patrolPointMapPosition.x, patrolPointMapPosition.y);
        // loop through positions in patrol points
        for (int i = 1; i < _patrolLoop.PatrolPoints.Length; i++) {
            patrolPointMapPosition = LevelDataManager.Instance.WorldToArraySpace(_patrolLoop.PatrolPoints[closestPointIndex].position);
            int distance = MapService.DistanceFromStart(startX, startY, patrolPointMapPosition.x, patrolPointMapPosition.y);
            if (distance < closestDistance) {
                closestPointIndex = i;
            }
        }
        return closestPointIndex;
    }

    private void OnArrivedDestination() {
        _currentPatrolLoopIndex++;
        if (_currentPatrolLoopIndex >= _patrolLoop.PatrolPoints.Length) {
            _currentPatrolLoopIndex = 0;
        }
        _isHolding = true;
        _currentHoldTime = 0f;
        SetReadyToTransition(_onArrivedAtPatrolPoint);
    }

    private void SetNextDestination() {
        Transform nextPoint = _patrolLoop.PatrolPoints[_currentPatrolLoopIndex];
        IntVector3 nextPointMapPosition = LevelDataManager.Instance.WorldToArraySpace(nextPoint.position);
        PathStatus pathStatus = _unit.Navigator.SetDestination(_unit.MoveController.MapPosition, nextPointMapPosition);
        if (pathStatus == PathStatus.Invalid) {
            SetReadyToTransition(_onFailedToPath);
            return;
        }
    }
}
