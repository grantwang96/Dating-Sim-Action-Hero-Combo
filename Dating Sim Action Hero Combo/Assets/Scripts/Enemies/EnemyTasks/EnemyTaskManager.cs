using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTaskManager : MonoBehaviour {

    public static EnemyTaskManager Instance;
    [SerializeField] private EnemyTask _currentTask; // holds the current task for the enemies
    public EnemyTask currentTask {
        get { return _currentTask; }
        set {
            _currentTask = value;
        }
    }

    public Transform[] spawnpoints; // the list of spawnpoints for enemy units

    private void Awake() {
        Instance = this;
    }
}

