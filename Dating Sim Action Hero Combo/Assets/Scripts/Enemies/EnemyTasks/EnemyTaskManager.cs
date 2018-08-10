using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyTaskManager : MonoBehaviour {

    public static EnemyTaskManager Instance;
    [SerializeField] private EnemyTask _currentTask; // holds the current task for the enemies
    public EnemyTask currentTask {
        get { return _currentTask; }
        set {
            if(value == null) {
                if (_currentTask.successful) { Debug.Log("Game over!"); }
                else { Debug.Log("You win!"); }
            }
            _currentTask = value;
        }
    }

    public delegate void EnemyDeath(Damageable enemy);
    public EnemyDeath OnEnemyDeath;

    public Transform[] spawnpoints; // the list of spawnpoints for enemy units

    private void Awake() {
        Instance = this;
        OnEnemyDeath += currentTask.OnEnemyDeath;
    }

    public void EnemyKilled(Damageable enemy) {
        OnEnemyDeath.Invoke(enemy);
    }
}

