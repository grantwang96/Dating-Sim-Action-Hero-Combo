using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Enemies do nothing until provoked. Used in elimination missions.
/// </summary>
public class ETask_Idle : EnemyTask {

    [SerializeField] private List<Transform> points = new List<Transform>();

    private void Awake() {
        foreach(Transform child in transform) { points.Add(child); }
    }

    public override Vector2 GetLocation() {
        return nearestOpenSpace();
    }

    private Vector2 nearestOpenSpace() {
        for(int i = 0;i < points.Count; i++) {
            Transform temp = points[i];
            int rand = Random.Range(0, points.Count);
            points[i] = points[rand];
            points[rand] = temp;
        }
        for(int i = 0; i < points.Count; i++) {
            int x = GameManager.GetGridSpaceX(points[i].position.x);
            int y = GameManager.GetGridSpaceY(points[i].position.y);
            if(GameManager.Instance.grid[x, y] == null) {
                Debug.Log(points[i] + " " + x + ", " + y);
                Debug.Log("Space empty: " + GameManager.Instance.grid[x, y] == null);
                return new Vector2(x, y);
            }
        }
        return new Vector2(-1, -1);
    }

    public override void OnEnemyDeath(Damageable enemy) {
        Debug.Log(enemy.name);
        if (enemiesRequired.Contains(enemy)) {
            enemiesRequired.Remove(enemy);
        }
        if(enemiesRequired.Count == 0) { OnFailed(); }
    }

    public override void OnSucceed() {
        _successful = true;
        try {
            EnemyTaskManager.Instance.currentTask = NextTaskSuccess;
        } catch {
            Debug.LogError("ENEMY TASK MANAGER INSTANCE IS NULL!");
        }
    }

    public override void OnFailed() {
        _successful = false;
        try {
            EnemyTaskManager.Instance.currentTask = NextTaskFail;
        } catch {
            Debug.LogError("ENEMY TASK MANAGER INSTANCE IS NULL!");
        }
    }

    public override BrainState PerformAction() {
        Debug.Log("Do nothing.");
        return new GruntIdle();
    }
}
