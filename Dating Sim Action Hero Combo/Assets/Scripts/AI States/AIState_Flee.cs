using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Flee : AIState
{
    [SerializeField] private AIState _onArrivedDestination;
    [SerializeField] private int _fleeRadius;
    
    protected override void OnEnter() {
        base.OnEnter();
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
        Vector3 worldPointOfInterest = LevelDataManager.Instance.ArrayToWorldSpace(_unit.Navigator.PointOfInterest.x, _unit.Navigator.PointOfInterest.y);
        Vector3 direction = (_unit.MoveController.Body.position - worldPointOfInterest).normalized; // get opposite direction
        Vector3 worldTargetFleePoint = _unit.MoveController.Body.position + direction * _fleeRadius;
        IntVector3 targetFleePoint = LevelDataManager.Instance.WorldToArraySpace(worldTargetFleePoint);
        List<IntVector3> availableTiles = MapService.GetTraversableTiles(
            _fleeRadius, targetFleePoint, _unit, _unit.UnitData.TraversableThreshold, 1);
        if (availableTiles.Count == 0) {
            return _unit.MoveController.MapPosition;
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
    
    protected override void OnExit() {
        base.OnExit();
        _unit.Navigator.OnArrivedFinalDestination -= OnArrivedDestination;
    }

    private void OnArrivedDestination() {
        _unit.Navigator.OnArrivedFinalDestination -= OnArrivedDestination;
        SetReadyToTransition(_onArrivedDestination);
    }
}
