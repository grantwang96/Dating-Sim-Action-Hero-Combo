using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Enemies do nothing until provoked. Used in elimination missions.
/// </summary>
public class ETask_Idle : EnemyTask {

    [SerializeField] private List<Transform> points = new List<Transform>();

    private void Start() {
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
                return new Vector2(x, y);
            }
        }
        return new Vector2(-1, -1);
    }

    public override void OnSucceed() {
        if(NextTaskSuccess == null) { Debug.Log("Gameover!"); }
        try {
            EnemyTaskManager.Instance.currentTask = NextTaskSuccess;
        } catch {
            Debug.LogError("ENEMY TASK MANAGER INSTANCE IS NULL!");
        }
    }

    public override void OnFailed() {
        if (NextTaskFail == null) { Debug.Log("Gameover!"); }
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
