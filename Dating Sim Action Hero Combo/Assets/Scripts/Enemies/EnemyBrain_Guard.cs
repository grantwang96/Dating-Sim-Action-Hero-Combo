using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain_Guard : EnemyBrain {

    [SerializeField] public Transform pathSet;
    [SerializeField] public List<Transform> patrolPath = new List<Transform>();
    private int _pathIndex = 0; // where is your next destination
    public int pathIndex { get { return _pathIndex; } }

    private void Awake() {
        foreach(Transform child in pathSet) { patrolPath.Add(child); }
        transform.position = patrolPath[0].position;
    }

    protected override void Start() {
        if (!enemies.Contains(PlayerDamageable.Instance)) { enemies.Add(PlayerDamageable.Instance); }
        myDamageable = GetComponent<Damageable>();
        myCharMove = GetComponent<CharacterMove>();

        _currentClip = heldWeapon.clipCapacity;
        ChangeStates(new EnemyGuard_Patrol());
    }

    public override void React(Transform target) {
        currentTarget = target;
        if(currentState.GetType() == typeof(EnemyGuard_Aggro)) { return; }
        ChangeStates(new EnemyGuard_Aggro());
    }

    public void IncrementPathIndex() {
        _pathIndex++;
        if(_pathIndex >= patrolPath.Count) { _pathIndex = 0; }
    }

    // gets the closest point in the patrol path
    public Transform GetNearestPatrolPathPoint() {
        Transform point = null;
        try {
            point = patrolPath[0];
            for(int i = 0; i < patrolPath.Count; i++) {
                if(Vector2.Distance(transform.position, patrolPath[i].position) < Vector2.Distance(transform.position, point.position)) {
                    point = patrolPath[i];
                }
            }
        } catch {
            Debug.LogError(name + " DOESN'T HAVE A PATH!");
        }
        if (patrolPath.Contains(point)) {
            _pathIndex = patrolPath.IndexOf(point);
        }
        return point;
    }
}
