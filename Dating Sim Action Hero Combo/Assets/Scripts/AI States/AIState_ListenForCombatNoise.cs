using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_ListenForCombatNoise : AIState
{
    [SerializeField] private AIState _onCombatNoiseHeard;
    [SerializeField] private UnitSoundListener _unitSoundListener;
    
    protected override void OnEnter() {
        base.OnEnter();
        _unitSoundListener.OnCombatNoiseHeard += OnCombatNoiseHeard;
    }
    
    protected override void OnExit() {
        base.OnExit();
        _unitSoundListener.OnCombatNoiseHeard -= OnCombatNoiseHeard;
    }

    private void OnCombatNoiseHeard(IntVector3 position, Unit unit) {
        _unit.TargetManager.TrySetTarget(unit);
        _unit.Navigator.PointOfInterest = position;
        SetReadyToTransition(_onCombatNoiseHeard);
    }
}
