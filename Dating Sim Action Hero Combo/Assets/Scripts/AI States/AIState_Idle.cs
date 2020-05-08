using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Idle : AIState {

    [SerializeField] private float _minimumIdleTime;
    [SerializeField] private float _maximumIdleTime;

    private float _duration;
    private float _currentTime;

    public override void Enter(AIStateInitializationData initData = null) {
        _duration = Random.Range(_minimumIdleTime, _maximumIdleTime);
        _currentTime = 0f;
        base.Enter(initData);
    }

    public override bool Execute() {
        base.Execute();
        return IncrementTime();
    }

    private bool IncrementTime() {
        _currentTime += Time.deltaTime;
        if (_currentTime >= _duration) {
            SetNextTransition(AIStateTransitionId.OnIdleFinished);
            return true;
        }
        return false;
    }
}
