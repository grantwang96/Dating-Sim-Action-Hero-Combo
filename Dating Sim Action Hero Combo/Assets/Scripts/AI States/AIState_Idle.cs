using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIState_Idle : AIState {

    [SerializeField] private float _minimumIdleTime;
    [SerializeField] private float _maximumIdleTime;

    private float _duration;
    private float _currentTime;

    [SerializeField] private AIState _onIdleComplete;

    public override void Enter(AIStateInitializationData initData = null) {
        _duration = Random.Range(_minimumIdleTime, _maximumIdleTime);
        _currentTime = 0f;
        base.Enter(initData);
    }

    public override void Execute() {
        base.Execute();
        IncrementTime();
    }

    private void IncrementTime() {
        _currentTime += Time.deltaTime;
        if (_currentTime >= _duration) {
            SetReadyToTransition(_onIdleComplete);
            return;
        }
    }
}
