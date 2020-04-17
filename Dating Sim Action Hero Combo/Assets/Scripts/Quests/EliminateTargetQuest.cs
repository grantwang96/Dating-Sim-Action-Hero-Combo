using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Eliminate Target")]
public class EliminateTargetQuest : Quest
{
    [SerializeField] private string _unitId;
    [SerializeField] private string _unitType;
    [SerializeField] private Vector2 _location;

    public override void Begin() {
        base.Begin();

        EnemyManager.Instance.SpawnEnemy(_location, _unitType, _unitId);
        EnemyManager.Instance.OnEnemyDefeated += OnEnemyDefeated;
    }

    private void OnEnemyDefeated(IUnitController unit) {
        if (!unit.UnitId.Equals(_unitId)) {
            return;
        }
        EnemyManager.Instance.OnEnemyDefeated -= OnEnemyDefeated;
        FireOnComplete();
    }
}
