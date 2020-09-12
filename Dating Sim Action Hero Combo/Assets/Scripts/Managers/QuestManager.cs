using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : IInitializableManager
{
    public static QuestManager Instance { get; private set; }

    public QuestState CurrentQuestState { get; private set; }

    public event Action OnCurrentQuestUpdated;
    public event Action<QuestState> OnCurrentQuestCompleted;
    public event Action OnAllQuestsCompleted;

    private readonly List<Quest> _questList = new List<Quest>();
    public IReadOnlyList<Quest> QuestList => _questList;
    private int _currentQuestIndex;

    public void Initialize(Action<bool> initializationCallback = null) {
        Instance = this;
        InitializeQuestList();
        GameManager.Instance.OnGameStarted += OnGameStart;
        GameManager.Instance.OnGameEnded += OnGameEnd;
        CustomLogger.Log(nameof(QuestManager), $"Initializing {nameof(QuestManager)}");
        initializationCallback?.Invoke(true);
    }

    public void Dispose() {
        Instance = null;
        GameManager.Instance.OnGameStarted -= OnGameStart;
        GameManager.Instance.OnGameEnded -= OnGameEnd;
    }

    private void InitializeQuestList() {
        _questList.Clear();
        _questList.AddRange(GameLevelDataController.Instance.CurrentGameLevelData.QuestDatas);
    }

    private void OnGameStart() {
        _currentQuestIndex = 0;
        InitializeCurrentQuest();
        CustomLogger.Log(nameof(QuestManager), $"Game Started");
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
        Quest nextQuest = _questList[_currentQuestIndex];
        CurrentQuestState = nextQuest.Begin();
        if(CurrentQuestState == null) {
            CustomLogger.Error(nameof(QuestManager), $"Failed to generate quest state from {nextQuest.name}");
            return;
        }
        CurrentQuestState.OnCompleted += OnQuestCompleted;
        CurrentQuestState.OnFailed += OnQuestFailed;
        CustomLogger.Log(nameof(QuestInfoDisplayManager), "Updating current quest state");
        OnCurrentQuestUpdated?.Invoke();
    }

    private void UnsubscribeFromQuest() {
        CurrentQuestState.OnCompleted -= OnQuestCompleted;
        CurrentQuestState.OnFailed -= OnQuestFailed;
    }

    private void OnQuestCompleted() {
        CustomLogger.Log(nameof(QuestManager), $"Completed objective!");
        UnsubscribeFromQuest();
        OnCurrentQuestCompleted?.Invoke(CurrentQuestState);
        NextQuest();
    }

    private void OnQuestFailed() {
        CustomLogger.Log(nameof(QuestManager), $"Failed objective!");
        UnsubscribeFromQuest();
    }
}
