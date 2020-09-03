using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : IInitializableManager
{
    public static QuestManager Instance { get; private set; }

    public Quest CurrentQuest { get; private set; }

    public event Action<Quest> OnCurrentQuestCompleted;
    public event Action OnAllQuestsCompleted;

    private List<Quest> _questList = new List<Quest>();
    private int _currentQuestIndex;

    public void Initialize(Action<bool> initializationCallback = null) {
        Instance = this;
        GameManager.Instance.OnGameStarted += OnGameStart;
        GameManager.Instance.OnGameEnded += OnGameEnd;
        initializationCallback?.Invoke(true);
    }

    public void Dispose() {

    }

    private void OnGameStart() {
        _currentQuestIndex = 0;
        InitializeCurrentQuest();
    }

    private void OnGameEnd() {
        GameManager.Instance.OnGameStarted -= OnGameStart;
        GameManager.Instance.OnGameEnded -= OnGameEnd;
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
