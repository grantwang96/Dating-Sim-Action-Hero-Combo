using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Flee : AIState
{
    [SerializeField] private AIState _onArrivedDestination;
    [SerializeField] private int _fleeRadius;

    public override void Enter(AIStateInitializationData initData = null) {
        base.Enter(initData);
        SetDestination();
    }

    private void SetDestination() {
        // find a location away from the current hostile target
        IntVector3 targetDestination = GetFleeLocation();
        // listen to the move controller for pathing events
        _unit.Navigator.OnArrivedFinalDestination += OnArrivedDestination;
        // path to that safer location
        PathStatus pathStatus = _unit.Navigator.SetDestination(_unit.MoveController.MapPosition, targetDestination);
        if(pathStatus == PathStatus.Invalid) {
            OnArrivedDestination();
        }
    }

    private IntVector3 GetFleeLocation() {
        // get all available tiles within a radius
        List<IntVector3> availableTiles = MapService.GetTraversableTiles(
            _fleeRadius, _unit.MoveController.MapPosition, _unit, _unit.UnitData.TraversableThreshold, 1);
        if(availableTiles.Count == 0) {
            return _stateMachine.Unit.MoveController.MapPosition;
        }
        // iterate thru all the tiles and pick out the best one
        IntVector3 mapPosition = _unit.MoveController.MapPosition;
        Vector2 currentPosition = _unit.MoveController.Body.position;
        IntVector3 bestTile = availableTiles[0];
        int mapDistance = MapService.DistanceFromStart(mapPosition.x, mapPosition.y, bestTile.x, bestTile.y);
        Unit currentThreat = _unit.TargetManager.CurrentTarget;
        for (int i = 0; i < availableTiles.Count; i++) {
            // check if initial threat is within sight
            Vector2 targetTilePosition = LevelDataManager.Instance.ArrayToWorldSpace(availableTiles[i].x, availableTiles[i].y);
            float distanceFromThreat = Vector2.Distance(currentThreat.MoveController.Body.position, targetTilePosition);
            RaycastHit2D hit = Physics2D.Raycast(targetTilePosition, currentThreat.MoveController.Body.position, distanceFromThreat, _unit.TargetManager.VisionLayers);
            if(hit.transform != null) {
                // the threat is in sight, skip this tile
                if(hit.transform == currentThreat.MoveController.Body) {
                    continue;
                }
            }
            // compare distance
            int newMapDistance = MapService.DistanceFromStart(mapPosition.x, mapPosition.y, availableTiles[i].x, availableTiles[i].y);
            if (newMapDistance > mapDistance) {
                bestTile = availableTiles[i];
                mapDistance = newMapDistance;
            }
        }
        return bestTile;
    }

    public override void Exit(AIState nextState) {
        base.Exit(nextState);
        _unit.Navigator.OnArrivedFinalDestination -= OnArrivedDestination;
    }

    private void OnArrivedDestination() {
        _unit.Navigator.OnArrivedFinalDestination -= OnArrivedDestination;
        SetReadyToTransition(_onArrivedDestination);
    }
}
