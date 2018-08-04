using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianBrain : Brain {

    [SerializeField] private float panicRadius;

    protected override void Update() {
        base.Update();
    }

    public override void MainAction() {
        Debug.Log(name + "Merp");
    }

    public override void React(Transform target) {
        Debug.Log(transform.name + " is panicked!");
        Vector2 dir = target.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, panicRadius);
        Vector2 loc = (Vector2)transform.position + dir * panicRadius;
        try {
            loc = (Vector2)hit.transform.position + hit.normal;
        }
        catch {
            Debug.Log("No obstacles!");
        }
        Debug.Log(loc);
        myCharMove.SetDestination(Mathf.RoundToInt(loc.x), Mathf.RoundToInt(loc.y));
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if(collider.tag == "BulletSound") {
            React(collider.transform);
        }
    }
}
