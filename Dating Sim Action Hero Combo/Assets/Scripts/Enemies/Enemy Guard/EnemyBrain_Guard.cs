using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBrain_Guard : EnemyBrain {

    [SerializeField] public Transform pathSet;
    [SerializeField] public List<Transform> patrolPath = new List<Transform>();
    [SerializeField] private int _pathIndex = 0; // where is your next destination
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
        GetNearestPatrolPathPoint();
        ChangeStates(new Idle());
    }

    public override void ReactToThreat(Damageable target) {
        
        currentTarget = target;
        System.Type stateType = currentState.GetType();
        if(stateType == typeof(Threat_Detected)){ return; }
        // if (stateType == typeof(EnemyGuard_Aggro) || stateType == typeof(EnemyGuard_Alert)) { return; }
        ChangeStates(new Threat_Detected());
    }

    public override void OnThreatGone() {
        base.OnThreatGone();
        _xDest = currentTarget.XPos;
        _yDest = currentTarget.YPos;
        Debug.Log("Threat Last Seen: " + xDest + ", " + yDest);
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
