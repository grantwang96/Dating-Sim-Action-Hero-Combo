using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestStatus {
    NotStarted, Ongoing, Completed, Failed, Aborted
}

public abstract class Quest : ScriptableObject
{
    [SerializeField] protected string _questId;
    [SerializeField] protected string _questName;

    public string QuestId => _questId;
    public string QuestName => _questName;

    public abstract QuestState Begin();
}

public abstract class QuestState {

    public QuestStatus Status { get; protected set; }
    public abstract string QuestDescription { get; }

    public event Action OnStateUpdated;
    public event Action OnCompleted;
    public event Action OnAbort;
    public event Action OnFailed;

    private Quest _questData;

    public QuestState(Quest questData) {
        _questData = questData;
        Status = QuestStatus.Ongoing;
    }

    protected void FireOnComplete() {
        Status = QuestStatus.Completed;
        OnCompleted?.Invoke();
    }

    protected void FireOnFailed() {
        Status = QuestStatus.Failed;
        OnFailed?.Invoke();
    }

    protected void FireOnAbort() {
        Status = QuestStatus.Aborted;
        OnFailed?.Invoke();
    }

    protected void FireOnStateUpdated() {
        OnStateUpdated?.Invoke();
    }
}
