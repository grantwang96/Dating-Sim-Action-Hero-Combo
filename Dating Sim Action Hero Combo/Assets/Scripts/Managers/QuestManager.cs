using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestManager : IInitializableManager
{
    public static QuestManager Instance => GetOrSetInstance();
    public static QuestManager _instance;

    public QuestState CurrentMissionQuestState { get; private set; }
    public QuestState CurrentDateQuestState { get; private set; }
    public bool AllMissionQuestsCompleted { get; private set; }
    public bool AllDateQuestsCompleted { get; private set; }

    public event Action OnCurrentMissionQuestUpdated;
    public event Action<QuestState> OnCurrentMissionQuestCompleted;
    public event Action OnCurrentDateQuestUpdated;
    public event Action<QuestState> OnCurrentDateQuestCompleted;

    public event Action OnAllQuestsCompleted;

    private readonly List<Quest> _missionQuestList = new List<Quest>();
    private readonly List<Quest> _dateQuestList = new List<Quest>();
    private int _currentMissionQuestIndex;
    private int _currentDateQuestIndex;

    private static QuestManager GetOrSetInstance() {
        if(_instance == null) {
            _instance = new QuestManager();
        }
        return _instance;
    }

    public void Initialize(Action<bool> initializationCallback = null) {
        InitializeQuestList();
        GameEventsManager.StartGame?.Subscribe(OnGameStart);
        GameEventsManager.EndGame?.Subscribe(OnGameEnd);
        CustomLogger.Log(nameof(QuestManager), $"Initializing {nameof(QuestManager)}");
        AllMissionQuestsCompleted = false;
        initializationCallback?.Invoke(true);
    }

    public void Dispose() {
        GameEventsManager.StartGame?.Unsubscribe(OnGameStart);
        GameEventsManager.EndGame?.Unsubscribe(OnGameEnd);
    }

    private void InitializeQuestList() {
        _missionQuestList.Clear();
        _dateQuestList.Clear();
        _missionQuestList.AddRange(GameLevelDataController.Instance.CurrentGameLevelData.MissionQuestDatas);
        _dateQuestList.AddRange(GameLevelDataController.Instance.CurrentGameLevelData.DateQuestDatas);
    }

    private void OnGameStart() {
        _currentMissionQuestIndex = 0;
        _currentDateQuestIndex = 0;
        InitializeCurrentMissionQuest();
        InitializeDateQuest();
        CustomLogger.Log(nameof(QuestManager), $"Game Started");
    }

    private void OnGameEnd(EndGameContext endGameContext) {
        GameEventsManager.StartGame?.Unsubscribe(OnGameStart);
        GameEventsManager.EndGame?.Unsubscribe(OnGameEnd);
    }

    private void NextMissionQuest() {
        _currentMissionQuestIndex++;
        InitializeCurrentMissionQuest();
    }

    private void NextDateQuest() {
        _currentDateQuestIndex++;
        InitializeDateQuest();
    }

    private void InitializeCurrentMissionQuest() {
        if(_currentMissionQuestIndex >= _missionQuestList.Count) {
            AllMissionQuestsCompleted = true;
            CheckAllQuestsCompleted();
            return;
        }
        Quest nextQuest = _missionQuestList[_currentMissionQuestIndex];
        CurrentMissionQuestState = nextQuest.Begin();
        if(CurrentMissionQuestState == null) {
            CustomLogger.Error(nameof(QuestManager), $"Failed to generate quest state from {nextQuest.name}");
            return;
        }
        CurrentMissionQuestState.OnCompleted += OnMissionQuestCompleted;
        CurrentMissionQuestState.OnFailed += OnMissionQuestFailed;
        CustomLogger.Log(nameof(QuestInfoDisplayManager), $"Initialized next mission quest {CurrentMissionQuestState.QuestDescription}");
        OnCurrentMissionQuestUpdated?.Invoke();
    }

    private void InitializeDateQuest() {
        if(_currentDateQuestIndex >= _dateQuestList.Count) {
            AllDateQuestsCompleted = true;
            CheckAllQuestsCompleted();
            return;
        }
        Quest nextQuest = _dateQuestList[_currentDateQuestIndex];
        CurrentDateQuestState = nextQuest.Begin();
        if(CurrentDateQuestState == null) {
            CustomLogger.Error(nameof(QuestManager), $"Failed to generate quest state from {nextQuest.name}");
            return;
        }
        CurrentDateQuestState.OnCompleted += OnDateQuestCompleted;
        CurrentDateQuestState.OnFailed += OnDateQuestFailed;
        CustomLogger.Log(nameof(QuestInfoDisplayManager), $"Initialized next mission quest {CurrentDateQuestState.QuestDescription}");
        OnCurrentDateQuestUpdated?.Invoke();
    }

    private void CheckAllQuestsCompleted() {
        if(AllMissionQuestsCompleted && AllDateQuestsCompleted) {
            OnAllQuestsCompleted?.Invoke();
        }
    }

    private void UnsubscribeFromMissionQuest() {
        CurrentMissionQuestState.OnCompleted -= OnMissionQuestCompleted;
        CurrentMissionQuestState.OnFailed -= OnMissionQuestFailed;
    }

    private void UnsubscribeFromDateQuest() {
        CurrentDateQuestState.OnCompleted -= OnDateQuestCompleted;
        CurrentDateQuestState.OnFailed -= OnDateQuestFailed;
    }

    private void OnMissionQuestCompleted() {
        CustomLogger.Log(nameof(QuestManager), $"Completed quest {CurrentMissionQuestState.QuestDescription}!");
        UnsubscribeFromMissionQuest();
        OnCurrentMissionQuestCompleted?.Invoke(CurrentMissionQuestState);
        NextMissionQuest();
    }

    private void OnMissionQuestFailed() {
        CustomLogger.Log(nameof(QuestManager), $"Failed objective {CurrentMissionQuestState.QuestDescription}!");
        UnsubscribeFromMissionQuest();
    }

    private void OnDateQuestCompleted() {
        CustomLogger.Log(nameof(QuestManager), $"Completed quest {CurrentDateQuestState.QuestDescription}!");
        UnsubscribeFromDateQuest();
        OnCurrentDateQuestCompleted?.Invoke(CurrentDateQuestState);
        NextDateQuest();
    }

    private void OnDateQuestFailed() {
        CustomLogger.Log(nameof(QuestManager), $"Failed objective {CurrentDateQuestState.QuestDescription}!");
        UnsubscribeFromDateQuest();
    }
}
