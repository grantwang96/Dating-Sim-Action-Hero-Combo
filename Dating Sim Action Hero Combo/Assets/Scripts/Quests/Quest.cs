using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum QuestStatus {
    NotStarted, Ongoing, Completed, Failed, Aborted
}

[CreateAssetMenu(menuName = "Quest")]
public class Quest : ScriptableObject
{
    [SerializeField] protected string _questId;
    [SerializeField] protected string _questName;
    [SerializeField] protected string _questDescription;
    [SerializeField] protected List<QuestObjectiveData> _objectives = new List<QuestObjectiveData>();
    [SerializeField] private int _dateScoreReward;

    public string QuestId => _questId;
    public string QuestName => _questName;
    public string QuestDescription => _questDescription;
    public IReadOnlyList<QuestObjectiveData> Objectives => _objectives;
    public int DateScoreReward => _dateScoreReward;

    public QuestState Begin() {
        return new QuestState(this);
    }
}

public class QuestState {

    public QuestStatus Status { get; private set; }
    public string QuestName => _questData.QuestName;
    public string QuestDescription => _questData.QuestDescription;
    public IReadOnlyList<QuestObjectiveState> ObjectiveStates => _objectiveStates;
    public int DateScoreReward => _questData.DateScoreReward;

    public event Action OnStateUpdated;
    public event Action OnCompleted;
    public event Action OnAbort;
    public event Action OnFailed;

    protected readonly List<QuestObjectiveState> _objectiveStates = new List<QuestObjectiveState>();

    private Quest _questData;

    public QuestState(Quest questData) {
        _questData = questData;
        LoadObjectives();
        Status = QuestStatus.Ongoing;
    }

    private void LoadObjectives() {
        for (int i = 0; i < _questData.Objectives.Count; i++) {
            QuestObjectiveData objectiveData = _questData.Objectives[i];
            QuestObjectiveState objectiveState = objectiveData.CreateState();
            objectiveState.OnCompleted += OnObjectiveCompleted;
            objectiveState.OnFailed += OnObjectiveFailed;
            objectiveState.OnProgressUpdated += OnObjectiveProgressUpdated;
            _objectiveStates.Add(objectiveState);
        }
    }

    private void OnObjectiveCompleted(QuestObjectiveState objectiveState) {
        RemoveObjectiveState(objectiveState);
        TryCompleteQuest();
    }

    private void TryCompleteQuest() {
        for (int i = 0; i < _objectiveStates.Count; i++) {
            if (_objectiveStates[i].Status != QuestObjectiveStatus.Completed) {
                return;
            }
        }
        FireOnComplete();
    }

    private void OnObjectiveFailed(QuestObjectiveState objectiveState) {
        RemoveObjectiveState(objectiveState);
        FireOnFailed();
    }

    private void OnObjectiveProgressUpdated(QuestObjectiveState objectiveState) {
        OnStateUpdated?.Invoke();
    }

    private void RemoveObjectiveState(QuestObjectiveState objectiveState) {
        objectiveState.OnCompleted -= OnObjectiveCompleted;
        objectiveState.OnFailed -= OnObjectiveFailed;
        objectiveState.OnProgressUpdated -= OnObjectiveProgressUpdated;
        _objectiveStates.Remove(objectiveState);
    }

    private void FireOnComplete() {
        Status = QuestStatus.Completed;
        OnCompleted?.Invoke();
    }

    private void FireOnFailed() {
        Status = QuestStatus.Failed;
        OnFailed?.Invoke();
    }

    private void FireOnAbort() {
        Status = QuestStatus.Aborted;
        OnFailed?.Invoke();
    }
}
