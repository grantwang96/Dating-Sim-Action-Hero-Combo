using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private string _id;
    [SerializeField] private string _patrolLoopId;
    public string Id => _id;

    private void Awake() {
        RegisterSpawnpoint();
    }

    private void RegisterSpawnpoint() {
        LevelDataManager.Instance.RegisterEnemySpawn(_id, this);
    }

    public void Spawn(string prefabId, string uniqueId = "") {
        EnemySpawnData spawnData = new EnemySpawnData() {
            Position = transform.position,
            EnemyType = prefabId,
            OverrideId = uniqueId,
            PatrolLoopId = _patrolLoopId
        };
        EnemyManager.Instance.SpawnEnemy(spawnData);
    }
}
