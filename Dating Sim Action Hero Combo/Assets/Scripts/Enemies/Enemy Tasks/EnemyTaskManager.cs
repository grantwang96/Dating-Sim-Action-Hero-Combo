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
                GameManager.Instance.EndGame();
            } else {
                OnEnemyDeath -= _currentTask.OnEnemyDeath;
                OnEnemyDeath += value.OnEnemyDeath;
            }
            _currentTask = value;
        }
    }

    public delegate void EnemyDeath(Damageable enemy);
    public EnemyDeath OnEnemyDeath;

    public Transform[] spawnpoints; // the list of spawnpoints for enemy units

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        OnEnemyDeath += currentTask.OnEnemyDeath;
    }

    public void EnemyKilled(Damageable enemy) {
        OnEnemyDeath.Invoke(enemy);
    }
    
    /// <summary>
    /// Ends the game. Pass in the last enemy task to check whether player won or lost
    /// </summary>
    /// <param name="lastTask"></param>
    public void EndGame(EnemyTask lastTask) {
        // get result of game
        bool result = ValidateEndGame();

        // deactivate gameplayer PlayerInput controls
        PlayerInput.Instance.enabled = false;

        // display appropriate screen to result
        Debug.Log("Player has " + ((result) ? "won" : "lost") + "!");
    }

    /// <summary>
    /// Checks to see if player has won or lost
    /// </summary>
    /// <returns></returns>
    private bool ValidateEndGame() {
        if(PlayerDamageable.Instance.health > 0 && !currentTask.successful) {
            return true;
        }
        return false;
    }
}

