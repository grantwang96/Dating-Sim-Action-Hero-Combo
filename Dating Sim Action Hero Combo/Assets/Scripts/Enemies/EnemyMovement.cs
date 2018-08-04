using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : CharacterMove {

    private EnemyBrain brain;

    protected override void Start() {
        base.Start();

        brain = GetComponent<EnemyBrain>();
    }

    protected override void Update() {
        
    }
}
