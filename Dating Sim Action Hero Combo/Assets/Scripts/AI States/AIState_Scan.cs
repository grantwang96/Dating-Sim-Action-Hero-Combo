using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Scan")]
public class AIState_Scan : AIStateDataObject {

    protected override ActiveAIState GenerateActiveAIState(EnemyController enemyController) {
        ActiveScanState newState = new ActiveScanState(enemyController.Data, enemyController.Unit.transform);
        return newState;
    }
}

public class ActiveScanState : ActiveAIState {

    private float _visionAngle;
    private float _visionRange;
    private LayerMask _visionLayers;

    private Transform _unitTransform;

    public ActiveScanState(EnemyData enemyData, Transform unit) : base() {
        _visionAngle = enemyData.VisionAngle;
        _visionRange = enemyData.VisionRange;
        _visionLayers = enemyData.VisionLayers;
    }

    public override void OnExecute() {
        base.OnExecute();
        bool foundPlayer = Scan();
        if (foundPlayer) {
            SetNextTransition(AIStateTransitionId.OnUnitEnemySeen);
        }
    }

    private bool Scan() {
        bool foundPlayer = false;
        Vector2 unitPosition = _unitTransform.position;
        Vector2 playerPosition = PlayerUnit.Instance.Transform.position;
        float distance = Vector2.Distance(unitPosition, playerPosition);
        if (distance > _visionRange) {
            return foundPlayer;
        }
        Vector2 playerDirection = playerPosition - unitPosition;
        float angle = Vector2.Angle(_unitTransform.transform.forward, playerDirection);
        if (angle > _visionAngle) {
            return foundPlayer;
        }
        // player within range, make a raycast
        RaycastHit2D[] hit = new RaycastHit2D[1];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(_visionLayers);
        if (Physics2D.Raycast(unitPosition, playerDirection.normalized, filter, hit, distance) > 0) {
            // if this is the player set foundPlayer to true
            if (hit[0].transform == PlayerUnit.Instance.Transform) {
                foundPlayer = true;
            }
        }
        return foundPlayer;
    }
}
