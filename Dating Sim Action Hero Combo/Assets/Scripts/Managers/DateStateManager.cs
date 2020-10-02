﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateStateManager : IInitializableManager {

    private const string DateSpawnPointId = "DateSpawnPoint";

    public static DateStateManager Instance => GetOrSetInstance();
    private static DateStateManager _instance;

    public int DateRating { get; private set; }

    public event Action OnDateRatingUpdated;

    private DateData _dateData;

    private static DateStateManager GetOrSetInstance() {
        if(_instance == null) {
            _instance = new DateStateManager();
        }
        return _instance;
    }

    public void Initialize(Action<bool> initializationCallback = null) {
        DateRating = 0;

        QuestManager.Instance.OnCurrentDateQuestCompleted += OnDateQuestCompleted;
        GameEventsManager.StartGame.Subscribe(OnGameStart); // temp: remove this and use configurable actions to spawn date character
        _dateData = GameLevelDataController.Instance.CurrentGameLevelData.DateData;
        PooledObjectManager.Instance.RegisterPooledObject(_dateData.UnitPrefabId, 1);

        initializationCallback?.Invoke(true);
    }

    public void Dispose() {
        QuestManager.Instance.OnCurrentDateQuestCompleted -= OnDateQuestCompleted;
        GameEventsManager.StartGame.Unsubscribe(OnGameStart); // temp: remove this and use configurable actions
    }

    private void OnDateQuestCompleted(QuestState dateQuestState) {
        DateRating += dateQuestState.DateScoreReward;
        OnDateRatingUpdated?.Invoke();
    } 

    private void OnPlayerAgentSpotted() {
        // lose the game
    }

    private void OnGameStart() {
        SpawnDate();
    }

    private void SpawnDate() {
        if (!LevelDataManager.Instance.TryGetNPCSpawnPoint(DateSpawnPointId, out Transform spawnpoint)) {
            CustomLogger.Error(nameof(DateStateManager), $"Could not find spawnpoint with id \"{DateSpawnPointId}\"");
            return;
        }
        if (!PooledObjectManager.Instance.UsePooledObject(_dateData.UnitPrefabId, out PooledObject pooledObject)) {
            CustomLogger.Error(nameof(DateStateManager), $"Could not retrieve pooled object with id \"{_dateData.UnitPrefabId}\"");
            return;
        }
        DateUnit dateUnit = pooledObject as DateUnit;
        dateUnit.Initialize(_dateData.UnitPrefabId, _dateData);
        dateUnit.transform.position = spawnpoint.position;
        dateUnit.Spawn();
    }
}
