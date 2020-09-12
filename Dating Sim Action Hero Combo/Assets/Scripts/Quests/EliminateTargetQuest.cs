using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Eliminate Target")]
public class EliminateTargetQuest : Quest
{
    [SerializeField] private string _unitId;
    [SerializeField] private string _unitType;
    [SerializeField] private string _spawnId;

    public string UnitId => _unitId;
    public string UnitType => _unitType;
    public string SpawnId => _spawnId;

    public override QuestState Begin() {
        if (!LevelDataManager.Instance.TryGetEnemySpawn(_spawnId, out EnemySpawn spawn)) {
            CustomLogger.Error(nameof(EliminateTargetQuest), $"Could not retrieve enemy spawn point with id {_spawnId}!");
            return null;
        }
        return new EliminateTargetQuestState(this, spawn);
    }
}

public class EliminateTargetQuestState : QuestState {

    public override string QuestDescription => $"Eliminate {_targetUnitId}";

    private string _targetUnitId;

    public EliminateTargetQuestState(EliminateTargetQuest questData, EnemySpawn spawn) : base(questData) {
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
    }
}
