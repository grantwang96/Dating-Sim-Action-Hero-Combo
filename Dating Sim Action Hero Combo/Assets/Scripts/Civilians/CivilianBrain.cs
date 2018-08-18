﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianBrain : Brain {

    [SerializeField] private float panicRadius;
    [SerializeField] private bool occupied;

    protected override void Start() {
        currentState = new Civilian_Idle();
        base.Start();
    }

    protected override void Update() {
        if (occupied) { return; }
        base.Update();
    }

    public override bool CheckVision(Transform enemy) {
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position, enemy.position - transform.position, rangeOfVision, visionMask);
        Debug.DrawLine(transform.position, enemy.position, Color.blue, 10f);
        return rayhit.transform == enemy;
    }

    public override void MainAction() {
        Debug.Log(name + "Merp");
    }

    public override void React(Damageable target) {

        System.Type stateType = currentState.GetType();
        if (stateType == typeof(Civilian_RunAway) || stateType == typeof(Civilian_Panic)) { return; }
        currentTarget = target;
        ChangeStates(new Civilian_RunAway());
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if(collider.tag == "BulletSound") {
            BulletNoise bn = collider.GetComponent<BulletNoise>();
            React(bn.owner);
        }
    }
}
