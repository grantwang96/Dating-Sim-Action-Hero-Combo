using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTaskManager : MonoBehaviour {

    public static EnemyTaskManager Instance;

    public List<EnemyTask> enemyMission = new List<EnemyTask>(); // stores the list of tasks for the enemies

    public Transform[] spawnpoints; // the list of spawnpoints for enemy units

    private void Awake() {
        Instance = this;
    }
}

