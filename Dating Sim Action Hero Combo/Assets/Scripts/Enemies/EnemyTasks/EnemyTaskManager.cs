using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTaskManager : MonoBehaviour {

    public static EnemyTaskManager Instance;
    public EnemyTask currentTask; // holds the current task for the enemies
    public Transform[] spawnpoints; // the list of spawnpoints for enemy units

    private void Awake() {
        Instance = this;
    }
}

