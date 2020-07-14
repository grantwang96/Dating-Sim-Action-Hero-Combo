using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Scan : AIState {

    [SerializeField] private AIState _onHostileFoundState;
    
    public override void Execute() {
        base.Execute();
        bool foundHostile = ScanAll();
        if (foundHostile) {
            OnFoundHostile();
            return;
        }
        return;
    }

    private void OnFoundHostile() {
        SetReadyToTransition(_onHostileFoundState);
    }

    private bool ScanAll() {
        return _unit.TargetManager.GeneralScan();
    }
}