using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTask : MonoBehaviour { // stores information about the task
    
    public Vector2 location; // where the task is located
    public float timeToComplete; // how much time it takes to complete
    public float currentProgress; // the amount of time already put into the task(all start at 0)

    public EnemyDamageable[] enemiesRequired;

    public Vector2 getClosestLocation() {
        return new Vector2();
    }
}
