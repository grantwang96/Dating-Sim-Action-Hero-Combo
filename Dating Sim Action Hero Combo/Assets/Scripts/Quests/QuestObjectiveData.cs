using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestObjectiveStatus {
    Invalid, Ongoing, Failed, Completed
}

public abstract class QuestObjectiveData : ScriptableObject
{
    [SerializeField] protected string _objectiveDescription;

    public string ObjectiveDescription => _objectiveDescription;

    public abstract QuestObjectiveState CreateState();
}

public abstract class QuestObjectiveState {

    public string ObjectiveDescription { get; }
    public QuestObjectiveStatus Status { get; protected set; }

    public event Action<QuestObjectiveState> OnFailed;
    public event Action<QuestObjectiveState> OnCompleted;
    public event Action<QuestObjectiveState> OnProgressUpdated;

    public QuestObjectiveState(QuestObjectiveData data) {
        Status = QuestObjectiveStatus.Ongoing;
        ObjectiveDescription = data.ObjectiveDescription;
    }
    
    protected void FireOnComplete() {
        Status = QuestObjectiveStatus.Completed;
        OnCompleted?.Invoke(this);
    }

    protected void FireOnFailed() {
        Status = QuestObjectiveStatus.Failed;
        OnFailed?.Invoke(this);
    }

    protected void FireProgressUpdated() {
        OnProgressUpdated?.Invoke(this);
    }
}
