using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTargetManager : MonoBehaviour
{
    public Unit CurrentTarget { get; protected set; }

    [SerializeField] private LayerMask _scanLayers;

    public bool GeneralScan() {
        return false;
    }

    public bool ScanForHostile(Unit target) {
        return false;
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
