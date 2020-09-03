using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NPCUIDisplayManager : IInitializableManager
{
    private const string NPCUIDisplayPrefabId = "prefab.ui_NPCDisplay";
    private Dictionary<Unit, NPCUIDisplay> _registeredUnits = new Dictionary<Unit, NPCUIDisplay>();

    public void Initialize(Action<bool> initializationCallback = null) {

        EnemyManager.Instance.OnEnemySpawned += OnEnemySpawned;

        initializationCallback?.Invoke(true);
    }

    public void Dispose() {
        ClearAllDisplays();
    }

    private void OnEnemySpawned(Unit enemy) {
        if(!PooledObjectManager.Instance.UsePooledObject(NPCUIDisplayPrefabId, out PooledObject obj)) {
            if(!PooledObjectManager.Instance.RegisterPooledObject(NPCUIDisplayPrefabId, 20)) {
                CustomLogger.Error(nameof(NPCUIDisplayManager), $"Failed to register object with id {NPCUIDisplayPrefabId}");
                return;
            }
            OnEnemySpawned(enemy);
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

    private void ClearAllDisplays() {
        foreach (Unit unit in _registeredUnits.Keys) {
            _registeredUnits[unit].Despawn();
            PooledObjectManager.Instance.ReturnPooledObject(NPCUIDisplayPrefabId, _registeredUnits[unit]);
        }
        _registeredUnits.Clear();
    }
}
