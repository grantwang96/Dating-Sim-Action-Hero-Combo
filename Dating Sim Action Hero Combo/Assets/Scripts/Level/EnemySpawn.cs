using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private string _id;
    public string Id => _id;

    private void Awake() {
        RegisterSpawnpoint();
    }

    private void RegisterSpawnpoint() {
        LevelDataManager.Instance.RegisterEnemySpawn(_id, this);
    }

    public void Spawn(string prefabId, string uniqueId = "") {
        EnemyManager.Instance.SpawnEnemy(transform.position, prefabId, uniqueId);
    }
}
