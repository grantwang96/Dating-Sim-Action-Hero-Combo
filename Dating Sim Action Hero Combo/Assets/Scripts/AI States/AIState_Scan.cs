using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Scan : AIState {

    [SerializeField] private UnitTags _scanForTags;
    [SerializeField] private DetectableTags _detectablesToScanFor;
    [SerializeField] private AIState _onHostileFoundState;
    [SerializeField] private FieldOfViewVisualizer _visualizedFieldOfView;

    private List<Unit> _detectedUnits = new List<Unit>();

    public override void Enter(AIStateInitializationData initData = null) {
        base.Enter(initData);
        _visualizedFieldOfView.SetActive(true);
        _unit.TargetManager.OnCurrentTargetSet += OnCurrentTargetSet;
    }

    public override void Execute() {
        base.Execute();
        // ScanAll();
        GeneralScan();
    }

    public override void Exit(AIState nextState) {
        base.Exit(nextState);
        _visualizedFieldOfView.SetActive(false);
        _unit.TargetManager.OnCurrentTargetSet -= OnCurrentTargetSet;
    }

    private void OnFoundHostile() {
        SetReadyToTransition(_onHostileFoundState);
    }

    private void ScanAll() {
        _unit.TargetManager.GeneralScan(_detectedUnits, _scanForTags);
        if(_detectedUnits.Count > 0) {
            _unit.TargetManager.TrySetTarget(_detectedUnits[0]);
            OnFoundHostile();
        }
    }

    private void GeneralScan() {
        _unit.TargetManager.GeneralScan(_detectablesToScanFor);
    }

    private void OnCurrentTargetSet(Unit unit) {
        SetReadyToTransition(_onHostileFoundState);
    }
}