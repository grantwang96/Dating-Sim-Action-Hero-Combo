using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianBrain : Brain {

    [SerializeField] private float panicRadius;

    protected override void Update() {
        base.Update();
    }

    protected override void React(Vector2 dir) {
        Debug.Log(transform.name + " is panicked!");
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
            React((collider.transform.position - transform.position).normalized);
        }
    }
}
