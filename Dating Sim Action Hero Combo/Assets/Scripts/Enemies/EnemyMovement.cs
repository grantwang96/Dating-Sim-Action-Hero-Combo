using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : CharacterMove {

    [SerializeField] private EnemyData _enemyData;
    public EnemyData enemyData { get { return _enemyData; } }

    protected override void Start() {
        base.Start();
        _walkSpeed = _enemyData.walkSpeed;
        _runSpeed = _enemyData.runSpeed;
    }

    protected override void Update() {
        
    }
}
