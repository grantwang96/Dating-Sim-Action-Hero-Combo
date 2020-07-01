﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Move : AIState
{
    [SerializeField] private bool _fullSpeed;
    [SerializeField] private NPCNavigator _navigator;
    [SerializeField] private NPCMoveController _moveController;
    [SerializeField] private IntVector3 _nextDestination;

    [SerializeField] private AIState _onArrivedDestination;

    public override void Enter(AIStateInitializationData initData = null) {
        float speed = _fullSpeed ? _unit.UnitData.RunSpeed : _unit.UnitData.WalkSpeed;
        // Get path to next destination here
        if(_moveController == null) {
            return;
        }
        _nextDestination = _navigator.TargetPosition;
        _moveController.OnArrivedTargetDestination += OnArrivedTargetDestination;
        _moveController.SetDestination(speed, _nextDestination);
        base.Enter(initData);
    }
    
    private void OnArrivedTargetDestination() {
        _moveController.OnArrivedTargetDestination -= OnArrivedTargetDestination;
        SetReadyToTransition(_onArrivedDestination);
    }
}