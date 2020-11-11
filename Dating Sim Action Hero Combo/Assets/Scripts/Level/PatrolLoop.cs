using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolLoop : MonoBehaviour
{
    [SerializeField] private Transform[] _patrolPoints;
    public Transform[] PatrolPoints => _patrolPoints;

    private void Awake() {
        LevelDataManager.Instance.RegisterPatrolLoop(name, this);
    }
}
