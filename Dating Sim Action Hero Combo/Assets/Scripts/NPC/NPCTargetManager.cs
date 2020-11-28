using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCTargetManager : MonoBehaviour {
    private const int MaxFoundTargets = 15;
    public const float DetectionThreshold = 2;
    private const float MaxDetection = 5;

    public DetectedTarget CurrentDetectable { get; private set; }
    public DetectedTarget HighestDetectedTarget { get; private set; }
    public Unit CurrentTarget { get; private set; }

    public LayerMask TargetLayers => _targetLayers;
    public LayerMask VisionLayers => _visionLayers;
    public float VisionAngle => _visionAngle;
    public float VisionRange => _visionRange;

    public event Action<Unit> OnCurrentTargetSet;
    // on crime scene witnessesd

    [SerializeField] private Unit _unit;
    [SerializeField] private LayerMask _targetLayers;
    [SerializeField] private LayerMask _visionLayers;
    [SerializeField] private float _visionAngle;
    [SerializeField] private float _visionRange;

    private Collider2D[] _foundColliders = new Collider2D[MaxFoundTargets];

    private readonly Dictionary<IDetectable, DetectedTarget> _detectedTargets = new Dictionary<IDetectable, DetectedTarget>();

    public void Initialize() {
        _visionAngle = _unit.UnitData.VisionAngle;
        _visionRange = _unit.UnitData.VisionRange;
        _visionLayers = _unit.UnitData.VisionLayers;
    }

    public void GeneralScan(List<Unit> foundUnits, UnitTags unitTags) {
        int foundCollidersCount = Physics2D.OverlapCircleNonAlloc(_unit.MoveController.Body.position, _visionRange, _foundColliders, _targetLayers);
        foundUnits.Clear();
        for (int i = 0; i < foundCollidersCount; i++) {
            Collider2D collider = _foundColliders[i];
            Unit unit = collider.GetComponent<Unit>();
            if (unit != null && unit != _unit) {
                if (UnitUtils.ContainsTag(unit.UnitTags, unitTags) &&
                    Scan(unit, _unit.MoveController.Body.transform, _visionRange, _visionLayers, _visionAngle)) {
                    foundUnits.Add(unit);
                }
            }
        }
    }

    public void GeneralScan(DetectableTags detectableTags = 0) {
        // get all colliders within the vision radius
        int foundCollidersCount = Physics2D.OverlapCircleNonAlloc(_unit.MoveController.Body.position, _visionRange, _foundColliders, _targetLayers);
        for (int i = 0; i < foundCollidersCount; i++) {
            Collider2D collider = _foundColliders[i];
            IDetectable detectable = collider.GetComponent<IDetectable>();
            // ensure that this collider is a detectable
            if (detectable == null) {
                continue;
            }
            // ensure that this detectable matches at least one of the tags and can be seen
            bool containsDetectableTag = (detectable.DetectableTags & detectableTags) != 0;
            if (containsDetectableTag && Scan(detectable, _unit.MoveController.Body.transform, _visionRange, _visionLayers, _visionAngle)) {
                // add/update the detected target entry
                if (!_detectedTargets.TryGetValue(detectable, out DetectedTarget entry)) {
                    _detectedTargets.Add(detectable, new DetectedTarget() {
                        Target = detectable,
                        DetectionValue = 0f,
                    });
                }
                _detectedTargets[detectable].DetectedThisFrame = true;
            }
        }
        ProcessDetectedTargets();
    }

    public void ScanForCurrentTarget(IDetectable detectable) {
        ProcessDetectedTargetEntry(CurrentDetectable);
    }

    // this function searches for a specific target
    public bool CanSeeTarget(Unit target) {
        if(!IsWithinVisionRange(_visionRange, _unit.MoveController.Body.position, target.MoveController.Body.position)) {
            return false;
        }
        return ScanCast(target, target.MoveController.Body, _visionRange, _visionLayers);
    }

    // attempt to set the current unit target
    public void TrySetTarget(Unit target) {
        if (CanSeeTarget(target)) {
            OverrideCurrentTarget(target);
        }
    }

    // overrides the current target without scanning
    public void OverrideCurrentTarget(Unit newTarget) {
        CurrentTarget = newTarget;
        if (!_detectedTargets.TryGetValue(newTarget, out DetectedTarget entry)) {
            _detectedTargets.Add(newTarget, new DetectedTarget() {
                Target = newTarget,
                DetectedThisFrame = true,
                DetectionValue = MaxDetection
            });
        }
        CurrentDetectable = _detectedTargets[newTarget];
        HighestDetectedTarget = null;
        _detectedTargets.Clear();
        OnCurrentTargetSet?.Invoke(newTarget);
    }

    private void ProcessDetectedTargets() {
        // if there is a current focused target
        if (CurrentDetectable != null) {
            ProcessCurrentDetectable();
            return;
        }
        // check all detected targets
        var itemsToRemove = new List<DetectedTarget>();
        foreach (KeyValuePair<IDetectable, DetectedTarget> keyPair in _detectedTargets) {
            DetectedTarget entry = keyPair.Value;
            entry.DetectionValue += entry.DetectedThisFrame ? Time.deltaTime : -Time.deltaTime;
            // if this target has reached the detection threshold
            if (entry.DetectionValue >= DetectionThreshold) {
                DetectionThresholdHit(entry);
                break;
            }
            // if there is no highest or this detection value is higher
            if (HighestDetectedTarget == null || entry.DetectionValue > HighestDetectedTarget.DetectionValue) {
                HighestDetectedTarget = entry;
            }
            // if this has reached 0 detection
            if (entry.DetectionValue <= 0f && !entry.DetectedThisFrame) {
                itemsToRemove.Add(entry);
            }
            // reset the entry
            entry.DetectedThisFrame = false;
        }
        // remove all expired detection targets
        foreach (var item in itemsToRemove) {
            _detectedTargets.Remove(item.Target);
        }
    }

    // checks whether a given target is seen and updates their detection values accordingly
    private void ProcessDetectedTargetEntry(DetectedTarget entry) {
        entry.DetectedThisFrame = Scan(entry.Target, _unit.MoveController.Body.transform, _visionRange, _visionLayers, _visionAngle);
        entry.DetectionValue += entry.DetectedThisFrame ? Time.deltaTime : -Time.deltaTime;
        if (entry.DetectionValue > MaxDetection) {
            entry.DetectionValue = MaxDetection;
        }
    }

    // processes whether the current detectable can be seen
    private void ProcessCurrentDetectable() {
        ProcessDetectedTargetEntry(CurrentDetectable);
        if (CurrentDetectable.DetectionValue < DetectionThreshold) {
            CurrentDetectable = null;
        }
    }

    private void DetectionThresholdHit(DetectedTarget entry) {
        CurrentDetectable = entry;
        HighestDetectedTarget = null;
        CurrentDetectable.DetectionValue = MaxDetection;
        Unit unit = entry.Target as Unit;
        if (unit != null) {
            OverrideCurrentTarget(unit);
        }
        _detectedTargets.Clear();
    }

    // this performs the actual checks and raycast towards a given target
    private static bool Scan(Unit target, Transform unitTransform, float visionRange, LayerMask visionLayers, float visionAngle = 360f) {
        bool found = false;
        Vector2 unitPosition = unitTransform.position;
        Vector2 otherPosition = target.transform.position;
        // ensure the target distance is within range
        float distance = Vector2.Distance(unitPosition, otherPosition);
        if(!IsWithinVisionRange(visionRange, unitPosition, otherPosition)) {
            return found;
        }
        // ensure the target's direction is within the view cone
        Vector2 direction = otherPosition - unitPosition;
        float angle = Vector2.Angle(unitTransform.transform.up, direction);
        if (!IsWithinVisionAngle(visionAngle, unitTransform, otherPosition)) {
            return found;
        }
        // target within range, make a raycast
        RaycastHit2D[] hit = new RaycastHit2D[1];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(visionLayers);
        if (Physics2D.Raycast(unitPosition, direction.normalized, filter, hit, distance) > 0) {
            // if this is the hostile, set found to true
            if (hit[0].transform == target.transform) {
                found = true;
            }
        }
        return found;
    }

    private static bool Scan(IDetectable detectable, Transform unitTransform, float visionRange, LayerMask visionLayers, float visionAngle = 360f) {
        bool found = false;
        Vector2 unitPosition = unitTransform.position;
        Vector2 otherPosition = detectable.Transform.position;
        // ensure the target distance is within range
        float distance = Vector2.Distance(unitPosition, otherPosition);
        if (distance > visionRange) {
            return found;
        }
        // ensure the target's direction is within the view cone
        Vector2 direction = otherPosition - unitPosition;
        float angle = Vector2.Angle(unitTransform.up, direction);
        if (angle > visionAngle) {
            return found;
        }
        found = ScanCast(detectable, unitTransform, visionRange, visionLayers);
        return found;
    }

    private static bool ScanCast(IDetectable detectable, Transform unitTransform, float visionRange, LayerMask visionLayers) {
        bool found = false;
        Vector2 unitPosition = unitTransform.position;
        Vector2 otherPosition = detectable.Transform.position;
        float distance = Vector2.Distance(unitPosition, otherPosition);
        Vector2 direction = otherPosition - unitPosition;
        RaycastHit2D[] hit = new RaycastHit2D[1];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(visionLayers);
        if (Physics2D.Raycast(unitPosition, direction.normalized, filter, hit, distance) > 0) {
            // if this is the hostile, set found to true
            if (hit[0].transform == detectable.Transform) {
                found = true;
            }
        }
        return found;
    }

    private static bool IsWithinVisionRange(float visionRange, Vector3 originPosition, Vector3 targetPosition) {
        float distance = Vector2.Distance(originPosition, targetPosition);
        return distance <= visionRange;
    }

    private static bool IsWithinVisionAngle(float visionAngle, Transform unitTransform, Vector2 targetPosition) {
        Vector2 unitPosition = unitTransform.position;
        // ensure the target's direction is within the view cone
        Vector2 direction = targetPosition - unitPosition;
        float angle = Vector2.Angle(unitTransform.up, direction);
        return angle <= visionAngle;
    }
}

public class DetectedTarget {
    public IDetectable Target;
    public float DetectionValue;
    public bool DetectedThisFrame;
}