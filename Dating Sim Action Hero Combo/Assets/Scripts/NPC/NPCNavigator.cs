using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCNavigator : MonoBehaviour
{
    [SerializeField] private IntVector3 _targetPosition;

    public Vector2 MoveInput => GetMoveInput();
    public Vector2 LookInput => GetLookInput();
    public IntVector3 TargetPosition => _targetPosition;
    public Transform LookTarget { get; set; }

    [SerializeField] private Unit _unit;
    [SerializeField] private NPCMoveController _moveController;
    [SerializeField] private List<IntVector3> _currentPath = new List<IntVector3>();
    [SerializeField] private IntVector3 _currentDestination;
    [SerializeField] private bool _canPassThroughOccupants;
    private Vector2 _currentWorldDestination;
    private bool _isPathing;

    public event Action<IntVector3> OnTargetPositionSet;
    public event Action OnArrivedFinalDestination;

    // set the destination
    public PathStatus SetDestination(IntVector3 start, IntVector3 destination) {
        ClearDestination();
        PathStatus pathStatus = MapService.GetPathToDestination(
            start,
            destination,
            _currentPath,
            _unit,
            _unit.UnitData.TraversableThreshold
            );
        if (pathStatus == PathStatus.Invalid) {
            ArrivedFinalDestination();
            return PathStatus.Invalid;
        }
        _isPathing = true;
        _moveController.OnMapPositionUpdated += OnMapPositionUpdated;
        UpdateCurrentDestination();
        _targetPosition = destination;
        OnTargetPositionSet?.Invoke(destination);
        return PathStatus.Complete;
    }

    public void ClearDestination() {
        _isPathing = false;
        _currentPath.Clear();
        _moveController.OnMapPositionUpdated -= OnMapPositionUpdated;
    }

    // update to the next position in the current path
    private void UpdateCurrentDestination() {
        // if we've arrived, clean up
        if (_currentPath.Count == 0) {
            ArrivedFinalDestination();
            return;
        }
        // set the current destination
        _currentDestination = _currentPath[0];
        ITileInfo tileInfo = LevelDataManager.Instance.GetTileAt(_currentDestination.x, _currentDestination.y);
        // ensure that the current destination is currently traversable
        if(!_canPassThroughOccupants && tileInfo.Occupants.Count > 0) {
            ArrivedFinalDestination();
            return;
        }
        // set the world destination
        _currentWorldDestination = LevelDataManager.Instance.ArrayToWorldSpace(_currentPath[0].x, _currentPath[0].y);
        _currentPath.RemoveAt(0);
    }

    // When arriving at the final destination in the path
    private void ArrivedFinalDestination() {
        ClearDestination();
        OnArrivedFinalDestination?.Invoke();
    }

    private Vector2 GetMoveInput() {
        if (!_isPathing) {
            return Vector2.zero;
        }
        return (_currentWorldDestination - (Vector2)_moveController.Body.position).normalized;
    }

    // calculates the look input based on look target and pathing
    private Vector2 GetLookInput() {
        // check if there is a specific target the NPC should be looking at
        if(LookTarget != null) {
            return (LookTarget.position - _moveController.Body.position);
        }
        // check if the NPC is walking towards something specific
        else if (_isPathing) {
            return MoveInput;
        }
        // simply use the current rotation of the move controller otherwise
        return _moveController.Body.up;
    }

    // listen to character's map position updates
    private void OnMapPositionUpdated(IntVector3 newPosition) {
        if (_currentPath.Count == 0) {
            ArrivedFinalDestination();
            return;
        }
        if(newPosition == _currentDestination) {
            UpdateCurrentDestination();
        }
    }
}
