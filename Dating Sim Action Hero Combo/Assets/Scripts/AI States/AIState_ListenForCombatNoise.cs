using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_ListenForCombatNoise : AIState
{
    [SerializeField] private AIState _onCombatNoiseHeard;
    [SerializeField] private UnitSoundListener _unitSoundListener;

    public override void Enter(AIStateInitializationData initData = null) {
        base.Enter(initData);
        _unitSoundListener.OnCombatNoiseHeard += OnCombatNoiseHeard;
    }

    public override void Exit(AIState nextState) {
        base.Exit(nextState);
        _unitSoundListener.OnCombatNoiseHeard -= OnCombatNoiseHeard;
    }

    private void OnCombatNoiseHeard(IntVector3 position, Unit unit) {
        _unit.TargetManager.TrySetTarget(unit);
        _unit.Navigator.PointOfInterest = position;
        SetReadyToTransition(_onCombatNoiseHeard);
    }
}
