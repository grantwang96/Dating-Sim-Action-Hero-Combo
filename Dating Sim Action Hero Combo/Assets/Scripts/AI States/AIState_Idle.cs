using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI State/Idle")]
public class AIState_Idle : AIStateDataObject {

    [SerializeField] private float _minimumIdleTime;
    [SerializeField] private float _maximumIdleTime;

    protected override ActiveAIState GenerateActiveAIState(EnemyController enemyController) {
        float duration = Random.Range(_minimumIdleTime, _maximumIdleTime);
        ActiveIdleState newState = new ActiveIdleState(duration);
        return newState;
    }
}

public class ActiveIdleState : ActiveAIState {

    public readonly float Duration;
    public float CurrentTime { get; private set; }

    public ActiveIdleState(float duration) : base() {
        Duration = duration;
        CurrentTime = 0f;
    }

    public override void OnExecute() {
        base.OnExecute();
        IncrementTime();
    }

    private void IncrementTime() {
        CurrentTime += Time.deltaTime;
        if (CurrentTime >= Duration) {
            SetNextTransition(AIStateTransitionId.OnUnitIdleFinished);
        }
    }
}
