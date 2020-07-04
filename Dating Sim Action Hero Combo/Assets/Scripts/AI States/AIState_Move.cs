﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Move : AIState
{
    [SerializeField] private bool _fullSpeed;
    [SerializeField] private NPCNavigator _navigator;
    [SerializeField] private NPCMoveController _moveController;

    [SerializeField] private AIState _onArrivedDestination;

    public override void Enter(AIStateInitializationData initData = null) {
        float speed = _fullSpeed ? _unit.UnitData.RunSpeed : _unit.UnitData.WalkSpeed;
        // Get path to next destination here
        if(_moveController == null) {
            return;
        }
        _moveController.SetSpeed(speed);
        _navigator.OnArrivedFinalDestination += OnArrivedFinalDestination;
        _navigator.SetDestination(_moveController.MapPosition, _navigator.TargetPosition);
        base.Enter(initData);
    }
    
    private void OnArrivedFinalDestination() {
        _navigator.OnArrivedFinalDestination -= OnArrivedFinalDestination;
        SetReadyToTransition(_onArrivedDestination);
    }
}