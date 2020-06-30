using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Eliminate Target")]
public class EliminateTargetQuest : Quest
{
    [SerializeField] private string _unitId;
    [SerializeField] private string _unitType;
    [SerializeField] private string _spawnId;

    public override void Begin() {
        base.Begin();
        if(!LevelDataManager.Instance.TryGetEnemySpawn(_spawnId, out EnemySpawn spawn)) {
            CustomLogger.Error(nameof(EliminateTargetQuest), $"Could not retrieve enemy spawn point with id {_spawnId}!");
            FireOnAbort();
            return;
        }
        spawn.Spawn(_unitType, _unitId);
        EnemyManager.Instance.OnEnemyDefeated += OnEnemyDefeated;
    }

    private void OnEnemyDefeated(UnitController unit) {
        if (!unit.UnitId.Equals(_unitId)) {
            return;
        }
        EnemyManager.Instance.OnEnemyDefeated -= OnEnemyDefeated;
        FireOnComplete();
    }
}
