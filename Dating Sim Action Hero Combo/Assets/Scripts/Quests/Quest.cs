using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Quest : ScriptableObject
{

    public enum QuestStatus {
        NotStarted, Ongoing, Completed, Failed
    }

    public QuestStatus Status { get; protected set; }

    public event Action OnCompleted;
    public event Action OnFailed;

    public virtual void Begin() {
        Status = QuestStatus.Ongoing;
    }

    protected void FireOnComplete() {
        Status = QuestStatus.Completed;
        OnCompleted?.Invoke();
    }

    protected void FireOnFaile() {
        Status = QuestStatus.Failed;
        OnFailed?.Invoke();
    }
}
