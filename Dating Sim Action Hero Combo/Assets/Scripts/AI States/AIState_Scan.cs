using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Scan : AIState {

    [SerializeField] private NPCTargetManager _targetManager;
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
        if (_targetManager.GeneralScan()) {
            // TODO: set target in target manager
            return true;
        }
        return false;
    }
}

