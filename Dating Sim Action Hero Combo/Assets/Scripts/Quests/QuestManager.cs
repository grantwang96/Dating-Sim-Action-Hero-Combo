using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public Quest CurrentQuest { get; private set; }

    public event Action<Quest> OnCurrentQuestCompleted;
    public event Action OnAllQuestsCompleted;

    [SerializeField] private List<Quest> _questList = new List<Quest>();
    private int _currentQuestIndex;

    private void Awake() {
        Instance = this;
    }

    // TODO: figure out when and how the game will start
    // temp start the first quest immediately
    private void Start() {
        OnGameStart();
    }

    private void OnGameStart() {
        _currentQuestIndex = 0;
        InitializeCurrentQuest();
    }

    private void NextQuest() {
        _currentQuestIndex++;
        InitializeCurrentQuest();
    }

    private void InitializeCurrentQuest() {
        if(_currentQuestIndex >= _questList.Count) {
            OnAllQuestsCompleted?.Invoke();
            return;
        }
        CurrentQuest = _questList[_currentQuestIndex];
        CurrentQuest.OnCompleted += OnQuestCompleted;
        CurrentQuest.OnFailed += OnQuestFailed;
        CurrentQuest.Begin();
    }

    private void UnsubscribeFromQuest() {
        CurrentQuest.OnCompleted -= OnQuestCompleted;
        CurrentQuest.OnFailed -= OnQuestFailed;
    }

    private void OnQuestCompleted() {
        CustomLogger.Log(nameof(QuestManager), $"Completed objective!");
        UnsubscribeFromQuest();
        OnCurrentQuestCompleted?.Invoke(CurrentQuest);
        NextQuest();
    }

    private void OnQuestFailed() {
        CustomLogger.Log(nameof(QuestManager), $"Failed objective!");
        UnsubscribeFromQuest();
    }
}
