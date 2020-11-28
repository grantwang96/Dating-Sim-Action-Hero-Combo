﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Move : AIState
{
    [SerializeField] private bool _fullSpeed;
    [SerializeField] private NPCMoveController _moveController;

    [SerializeField] private AIState _onArrivedDestination;
    
    protected override void OnEnter() {
        base.OnEnter();
        float speed = _fullSpeed ? _stateMachine.Unit.UnitData.RunSpeed : _unit.UnitData.WalkSpeed;
        // Get path to next destination here
        if (_moveController == null) {
            return;
        }
        _moveController.SetSpeed(speed);
        _unit.Navigator.OnArrivedFinalDestination += OnArrivedFinalDestination;
        _unit.Navigator.SetDestination(_moveController.MapPosition, _unit.Navigator.TargetPosition);
    }

    private void OnArrivedFinalDestination() {
        _unit.Navigator.OnArrivedFinalDestination -= OnArrivedFinalDestination;
        SetReadyToTransition(_onArrivedDestination);
    }
}