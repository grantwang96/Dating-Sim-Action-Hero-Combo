using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianBrain : Brain {

    [SerializeField] private float panicRadius;
    [SerializeField] private bool occupied;

    protected override void Start() {
        currentState = new NPC_Idle();
        base.Start();
    }

    protected override void Update() {
        if (occupied) { return; }
        base.Update();
    }

    public override void MainAction() {
        Debug.Log(name + "Merp");
    }

    public override void React(Damageable target) {
        Debug.Log("Ah! " + target.name);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if(collider.tag == "BulletSound") {
            BulletNoise bn = collider.GetComponent<BulletNoise>();
            React(bn.owner);
        }
    }
}
