using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCUIDetectionDisplay : MonoBehaviour
{
    [SerializeField] private NPCUIDisplay _npcUIDisplay;
    [SerializeField] private FillBar _fillBar;

    private NPCUnit _unit => _npcUIDisplay.Unit;
    private NPCTargetManager _targetManager => _unit.TargetManager;

    private void Update() {
        SetDetectionDisplayValue();
    }

    private void SetDetectionDisplayValue() {
        bool showDisplay = false;
        if(_targetManager.CurrentDetectable != null) {
            _fillBar.gameObject.SetActive(showDisplay);
            return;
        }
        DetectedTarget highest = _targetManager.HighestDetectedTarget;
        showDisplay = highest != null && highest.DetectionValue > 0f;
        _fillBar.gameObject.SetActive(showDisplay);
        if (!showDisplay) {
            return;
        }
        float value = highest.DetectionValue / NPCTargetManager.DetectionThreshold;
        _fillBar.UpdateValueInstant(value);
    }
}
