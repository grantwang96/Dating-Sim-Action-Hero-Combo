using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Scan : AIState {
    
    [SerializeField] private float _visionAngle;
    [SerializeField] private float _visionRange;
    [SerializeField] private LayerMask _visionLayers;
    [SerializeField] private UnitTags _hostileTags;

    [SerializeField] private Transform _unitTransform;
    [SerializeField] private AIStateTransitionId _onHostileFound;

    private List<Unit> _hostiles;

    public override void Enter(AIStateInitializationData initData = null) {
        _visionAngle = _controller.Data.VisionAngle;
        _visionRange = _controller.Data.VisionRange;
        _visionLayers = _controller.Data.VisionLayers;
        _hostileTags = _controller.Data.HostileTags;
        _unitTransform = _controller.Unit.transform;
        CreateHostilesList();

        base.Enter(initData);
    }

    private void CreateHostilesList() {
        _hostiles = UnitsManager.Instance.GetUnitListByTags(_hostileTags);
    }

    public override bool Execute() {
        base.Execute();
        bool foundHostile = ScanAll();
        if (foundHostile) {
            OnFoundHostile();
            return true;
        }
        return foundHostile;
    }

    private void OnFoundHostile() {
        SetNextTransition(_onHostileFound);
    }

    private bool ScanAll() {
        bool foundHostile = false;
        for (int i = 0; i < _hostiles.Count; i++) {
            if (Scan(_hostiles[i], _unitTransform, _visionRange, _visionLayers, _visionAngle)) {
                _controller.FocusedTarget = _hostiles[i];
                foundHostile = true;
                break;
            }
        }
        return foundHostile;
    }

    public static bool Scan(Unit target, Transform unitTransform, float visionRange, LayerMask visionLayers, float visionAngle = 360f) {
        bool found = false;
        Vector2 unitPosition = unitTransform.position;
        Vector2 otherPosition = target.Transform.position;
        float distance = Vector2.Distance(unitPosition, otherPosition);
        if (distance > visionRange) {
            return found;
        }
        Vector2 direction = otherPosition - unitPosition;
        float angle = Vector2.Angle(unitTransform.transform.up, direction);
        if (angle > visionAngle) {
            return found;
        }
        // hostile within range, make a raycast
        RaycastHit2D[] hit = new RaycastHit2D[1];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(visionLayers);
        if (Physics2D.Raycast(unitPosition, direction.normalized, filter, hit, distance) > 0) {
            // if this is the hostile set found to true
            if (hit[0].transform == target.Transform) {
                found = true;
            }
        }
        return found;
    }
}

