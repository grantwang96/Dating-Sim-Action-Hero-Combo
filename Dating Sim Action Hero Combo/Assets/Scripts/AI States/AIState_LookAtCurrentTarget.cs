using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_LookAtCurrentTarget : AIState
{
    [SerializeField] private NPCTargetManager _npcTargetManager;
    [SerializeField] private NPCNavigator _navigator;
    [SerializeField] private AIState _onCantFindTarget;

    protected override void OnEnter() {
        base.OnEnter();
        if(_npcTargetManager.CurrentTarget == null) {
            Debug.LogError($"[{_unit.name}/{name}]: Current target was null!");
            SetReadyToTransition(_onCantFindTarget);
            return;
        }
        _navigator.LookTarget = _npcTargetManager.CurrentTarget.MoveController.Body;
    }
}
