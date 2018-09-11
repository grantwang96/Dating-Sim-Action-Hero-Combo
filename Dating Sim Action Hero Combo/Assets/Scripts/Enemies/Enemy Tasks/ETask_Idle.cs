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
        return NearestOpenSpace();
    }

    private Vector2 NearestOpenSpace() {
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
                Debug.Log(GameManager.Instance.grid[x, y]);
                return new Vector2(x, y);
            }
        }
        return new Vector2(-1, -1);
    }

    public override bool InValidSpace(int brainX, int brainY) {
        foreach(Transform point in points) {
            int gridX = GameManager.GetGridSpaceX(point.position.x);
            int gridY = GameManager.GetGridSpaceY(point.position.y);
            if(gridX == brainX && gridY == brainY) { return true; }
        }
        return false;
    }

    public override void OnEnemyDeath(Damageable enemy) {
        Debug.Log(enemy.name + " was killed!");
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

    public override bool PerformAction(Brain brain) {
        bool validSpace = InValidSpace(brain.xPos, brain.yPos);
        if (validSpace) {
            currentProgress += Time.deltaTime;
            if(currentProgress >= timeToComplete) { OnSucceed(); }
        };
        return validSpace;
    }
}
