using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "NPC/Enemy Guard")]
public class EGuard_Blueprint : Enemy_Blueprint {

    // guards go from idle to patrolling, rather than wandering
    public override void IdleEnter(Brain brain) {
        base.IdleEnter(brain);
    }
    public override IEnumerator Idling(Brain brain) {
        float time = 0f;
        float startingAngle = brain.transform.eulerAngles.z;
        while (time < 1f) {
            Transform target = brain.CheckVision(); // check vision for enemies
            if (target != null) {
                Damageable dam = target.GetComponent<Damageable>();
                if (dam && brain.Enemies.Contains(dam)) {
                    brain.ReactToThreat(dam);
                    yield break;
                }
            }
            time += Time.deltaTime * scanSpeed;
            brain.MyCharacterMove.SetRotation(Mathf.LerpAngle(startingAngle, startingAngle + scanRange, time));
            yield return null;
        }
        brain.ChangeStates(new CalculatePath_Wander());
    }

    // guards "wander" state is actually patrolling
    public override void CalculatePathWander(Brain brain) {
        try {
            EnemyBrain_Guard guardBrain = brain as EnemyBrain_Guard;
            Transform point = guardBrain.patrolPath[guardBrain.pathIndex];
            int x = GameManager.GetGridSpaceX(point.position.x);
            int y = GameManager.GetGridSpaceY(point.position.y);
            Debug.Log(x + ", " + y);
            brain.MyCharacterMove.SetDestination(x, y);
        } catch {
            Debug.LogError("ERROR CALCULATING PATH");
        }
    }

    public override void TravelExecute(Brain brain) {
        Transform target = brain.CheckVision(); // check vision for enemies
        if (target != null) {
            Damageable dam = target.GetComponent<Damageable>();
            if (dam && brain.Enemies.Contains(dam)) { brain.ReactToThreat(dam); }
        }

        // if we have finished moving to the destination
        if (brain.MyCharacterMove.movementRoutine == null) {
            try {
                EnemyBrain_Guard guard = brain as EnemyBrain_Guard;
                guard.IncrementPathIndex();
            } catch {
                Debug.Log("NOT AN ENEMY GUARD BRAIN");
            }
            brain.ChangeStates(new Idle());
        }
    }

    // guards directly address a threat without moving
    public override void ThreatDetectedEnter(Brain brain) {
        brain.MyCharacterMove.CancelDestination();
    }
    public override void ThreatDetectedExecute(Brain brain) {
        if (brain.CheckVision()) {
            Vector2 dir = brain.currentTarget.transform.position - brain.transform.position;
            brain.MyCharacterMove.SetRotation(dir);
            brain.MainAction();
        } else {
            brain.OnThreatGone();
            brain.ChangeStates(new CalculatePath_Point());
        }
    }
}
