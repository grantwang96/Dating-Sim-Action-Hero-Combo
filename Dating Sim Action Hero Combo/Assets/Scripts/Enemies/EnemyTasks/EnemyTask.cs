using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EnemyTask : MonoBehaviour { // stores information about the task
    
    public Vector2 location; // where the task is located(may not be at object location)
    public float timeToComplete; // how much time it takes to complete
    public float currentProgress; // the amount of time already put into the task(all start at 0)

    public EnemyDamageable[] enemiesRequired;

    [SerializeField] protected EnemyTask NextTaskSuccess; // the next task if the enemies succeed(gameover if null)
    [SerializeField] protected EnemyTask NextTaskFail; // the next task if the enemies fail(win if null)
    
    // if the enemies succeed in this task
    public abstract void OnSucceed();

    // if the enemies fail in this task
    public abstract void OnFailed();

    // gets the location for the enemy unit to go to
    public abstract Vector2 GetLocation();

    // what the enemy should do when they have reached the task location
    public abstract BrainState PerformAction();
}
