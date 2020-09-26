using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest Objective/Eliminate Target")]
public class EliminateTargetObjective : QuestObjectiveData
{
    [SerializeField] private string _unitId;
    [SerializeField] private string _unitType;
    [SerializeField] private string _spawnId;

    public string UnitId => _unitId;
    public string UnitType => _unitType;
    public string SpawnId => _spawnId;

    public override QuestObjectiveState CreateState() {
        if (!LevelDataManager.Instance.TryGetEnemySpawn(_spawnId, out EnemySpawn spawn)) {
            CustomLogger.Error(nameof(EliminateTargetObjective), $"Could not retrieve enemy spawn point with id {_spawnId}!");
            return null;
        }
        return new EliminateTargetQuestState(this, spawn);
    }
}

public class EliminateTargetQuestState : QuestObjectiveState {
    
    private string _targetUnitId;

    public EliminateTargetQuestState(EliminateTargetObjective questData, EnemySpawn spawn) : base(questData) {
        _targetUnitId = questData.UnitId;
        spawn.Spawn(questData.UnitType, _targetUnitId);
        EnemyManager.Instance.OnEnemyDefeated += OnEnemyDefeated;
    }
    
    private void OnEnemyDefeated(Unit unit) {
        if (!unit.UnitId.Equals(_targetUnitId)) {
            return;
        }
        EnemyManager.Instance.OnEnemyDefeated -= OnEnemyDefeated;
        FireOnComplete();
        FireProgressUpdated();
    }
}
