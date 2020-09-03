using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Quest : ScriptableObject
{
    public enum QuestStatus {
        NotStarted, Ongoing, Completed, Failed, Aborted
    }

    public QuestStatus Status { get; protected set; }

    public event Action OnCompleted;
    public event Action OnAbort;
    public event Action OnFailed;

    public virtual void Begin() {
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
}
