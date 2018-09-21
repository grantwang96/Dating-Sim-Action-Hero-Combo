using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianBrain : Brain {

    [SerializeField] private float panicRadius;
    [SerializeField] private bool occupied;

    protected override void Start() {
        ChangeStates(new Idle());
        base.Start();
    }

    protected override void Update() {
        if (occupied) { return; }
        base.Update();
    }

    public override bool CheckVision(Transform enemy) {
        RaycastHit2D rayhit = Physics2D.Raycast(transform.position, enemy.position - transform.position, myBluePrint.rangeOfVision, visionMask);
        Debug.DrawLine(transform.position, enemy.position, Color.blue, 10f);
        return rayhit.transform == enemy;
    }

    public override void MainAction() {

    }

    public override void ReactToThreat(Damageable target) {

        System.Type stateType = currentState.GetType();
        if(stateType == typeof(Threat_Detected)) { return; }
        // if (stateType == typeof(Civilian_RunAway) || stateType == typeof(Civilian_Panic)) { return; }
        currentTarget = target;
        // ChangeStates(new Civilian_RunAway());
        ChangeStates(new Threat_Detected());
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if(collider.tag == "BulletSound") {
            BulletNoise bn = collider.GetComponent<BulletNoise>();
            ReactToThreat(bn.owner);
        }
    }
    
    public override void ExitInteraction() {
        ChangeStates(new Idle());
    }

    public override void Interact() {
        ChangeStates(new Interaction());
        MainAction();
    }
}
