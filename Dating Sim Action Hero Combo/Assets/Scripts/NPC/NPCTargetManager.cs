using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTargetManager : MonoBehaviour
{
    private const int MaxFoundTargets = 15;

    public Unit CurrentTarget { get; private set; }

    [SerializeField] private Unit _unit;
    [SerializeField] private LayerMask _targetLayers;
    [SerializeField] private LayerMask _visionLayers;
    [SerializeField] private float _visionAngle;
    [SerializeField] private float _visionRange;

    private Collider2D[] _foundColliders = new Collider2D[MaxFoundTargets];

    private void Start() {
        _visionAngle = _unit.UnitData.VisionAngle;
        _visionRange = _unit.UnitData.VisionRange;
        _visionLayers = _unit.UnitData.VisionLayers;
    }

    public bool GeneralScan() {
        int foundCollidersCount = Physics2D.OverlapCircleNonAlloc(_unit.MoveController.Body.position, _visionRange, _foundColliders, _targetLayers);
        for(int i = 0; i < foundCollidersCount; i++) {
            Collider2D collider = _foundColliders[i];
            Unit unit = collider.GetComponent<Unit>();
            if(unit != null && unit != _unit) {
                if (Scan(unit, _unit.MoveController.Body.transform, _visionRange, _visionLayers, _visionAngle)) {
                    CurrentTarget = unit;
                    return true;
                }
            }
        }
        return false;
    }

    public bool ScanForHostile(Unit target) {
        return Scan(target, _unit.MoveController.Body.transform, _visionRange, _visionLayers, _visionAngle);
    }

    public void OverrideCurrentTarget(Unit newTarget) {
        CurrentTarget = newTarget;
    }

    public static bool Scan(Unit target, Transform unitTransform, float visionRange, LayerMask visionLayers, float visionAngle = 360f) {
        bool found = false;
        Vector2 unitPosition = unitTransform.position;
        Vector2 otherPosition = target.transform.position;
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
            if (hit[0].transform == target.transform) {
                found = true;
            }
        }
        return found;
    }
}
