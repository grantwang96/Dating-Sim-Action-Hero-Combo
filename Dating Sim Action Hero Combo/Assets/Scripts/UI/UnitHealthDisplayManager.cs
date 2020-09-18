﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCUIDisplayManager : IInitializableManager {
    public static NPCUIDisplayManager Instance => GetOrSetInstance();
    private static NPCUIDisplayManager _instance;

    private const string NPCUIDisplayPrefabId = "NPCDisplay";
    private Dictionary<Unit, NPCUIDisplay> _registeredUnits = new Dictionary<Unit, NPCUIDisplay>();

    private static NPCUIDisplayManager GetOrSetInstance() {
        if (_instance == null) {
            _instance = new NPCUIDisplayManager();
        }
        return _instance;
    }

    public void Initialize(Action<bool> initializationCallback = null) {

        EnemyManager.Instance.OnEnemySpawned += OnEnemySpawned;
        RegisterPooledObjects();
        initializationCallback?.Invoke(true);
    }

    private void RegisterPooledObjects() {
        if (!PooledObjectManager.Instance.RegisterPooledObject(NPCUIDisplayPrefabId, 20)) {
            CustomLogger.Error(nameof(NPCUIDisplayManager), $"Failed to register object with id {NPCUIDisplayPrefabId}");
        }
    }

    public void Dispose() {
        _registeredUnits.Clear();
        EnemyManager.Instance.OnEnemySpawned -= OnEnemySpawned;
        DeregisterPooledObjects();
    }

    private void DeregisterPooledObjects() {
        PooledObjectManager.Instance.DeregisterPooledObject(NPCUIDisplayPrefabId);
    }

    private void OnEnemySpawned(Unit enemy) {
        if(!PooledObjectManager.Instance.UsePooledObject(NPCUIDisplayPrefabId, out PooledObject obj)) {
            CustomLogger.Error(nameof(NPCUIDisplayManager), $"Could not get NPC info display object with id {NPCUIDisplayPrefabId}!");
            return;
        }
        NPCUIDisplay npcUIDisplay = obj as NPCUIDisplay;
        if (npcUIDisplay == null) {
            CustomLogger.Error(nameof(NPCUIDisplayManager), $"Did not receive a {nameof(NPCUIDisplay)} object!");
            return;
        }
        NPCUIDisplayInitializationData initData = new NPCUIDisplayInitializationData() {
            Unit = enemy
        };
        npcUIDisplay.Initialize(initData);
        npcUIDisplay.Spawn();
        enemy.OnUnitDefeated += OnEnemyDefeated;
        _registeredUnits.Add(enemy, npcUIDisplay);
    }

    private void OnEnemyDefeated(Unit enemy) {
        if(_registeredUnits.TryGetValue(enemy, out NPCUIDisplay display)) {
            display.Despawn();
            PooledObjectManager.Instance.ReturnPooledObject(NPCUIDisplayPrefabId, display);
            _registeredUnits.Remove(enemy);
            enemy.OnUnitDefeated -= OnEnemyDefeated;
        }
    }
}
