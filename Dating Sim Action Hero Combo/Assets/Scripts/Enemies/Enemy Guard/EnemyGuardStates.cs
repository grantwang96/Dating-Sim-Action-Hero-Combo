using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy guard patrols to next path point in route
/// </summary>
public class EnemyGuard_Patrol : BrainState {

    EnemyBrain_Guard guardBrain;
    bool searching = true;

    public override void Enter(Brain newBrain) {
        base.Enter(newBrain);
        if(newBrain.GetType() == typeof(EnemyBrain_Guard)) {
            guardBrain = (EnemyBrain_Guard)newBrain;
            Vector2 pos = guardBrain.patrolPath[guardBrain.pathIndex].position;
            int x = GameManager.GetGridSpaceX(pos.x);
            int y = GameManager.GetGridSpaceY(pos.y);
            guardBrain.MyCharacterMove.SetDestination(x, y);
        }
    }

    public override void Execute() {
        base.Execute();
        if(guardBrain.MyCharacterMove.movementRoutine == null) {
            if (searching) {
                searching = false;
                guardBrain.MyCharacterMove.MoveToDestination(myBrain.MyCharacterMove.walkSpeed);
            } else {
                guardBrain.IncrementPathIndex();
                guardBrain.MyCharacterMove.SetRotation(guardBrain.patrolPath[guardBrain.pathIndex].up);
                guardBrain.ChangeStates(new EnemyGuard_Scan());
                return;
            }
        }
        if (!searching) {
            guardBrain.MyCharacterMove.SetRotation(guardBrain.MyCharacterMove.currentDestination - (Vector2)guardBrain.transform.position);
        }
    }

    public override void Exit() {
        base.Exit();
        guardBrain.MyCharacterMove.SetRotation(guardBrain.patrolPath[guardBrain.pathIndex].up);
    }
}

/// <summary>
/// Enemy guard scans area for threats
/// </summary>
public class EnemyGuard_Scan : BrainState {
    private Coroutine lookRoutine;
    private float speed = .5f;
    private float range = 80f;

    public override void Enter(Brain brain) {
        base.Enter(brain);
        lookRoutine = myBrain.StartCoroutine(lookAround(brain.transform.eulerAngles.z));
    }

    public override void Execute() {
        base.Execute();
        if (lookRoutine == null) {
            myBrain.ChangeStates(new EnemyGuard_Patrol());
        }
    }

    public override void Exit() {
        if (lookRoutine != null) { myBrain.StopCoroutine(lookRoutine); }
    }

    private IEnumerator lookAround(float startingAngle) {
        float time = 0f;
        while (time < 1f) {
            myBrain.transform.eulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(startingAngle, startingAngle + range, time));
            time += Time.deltaTime * speed;
            yield return null;
        }
        time = 0f;
        while (time < 1f) {
            myBrain.transform.eulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(startingAngle + range, startingAngle - range, time));
            time += Time.deltaTime * speed / 2;
            yield return null;
        }
        time = 0f;
        while (time < 1f) {
            myBrain.transform.eulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(startingAngle - 60f, startingAngle, time));
            time += Time.deltaTime * speed;
            yield return null;
        }
        lookRoutine = null;
    }
}

/// <summary>
/// Enemy guard has seen a threat
/// </summary>
public class EnemyGuard_Alert : BrainState {

    float time = 0f;
    private Vector2 targetLastSpotted;

    public override void Enter(Brain brain)
    {
        base.Enter(brain);
        Debug.Log("Alert!");
    }

    public override void Execute() {
        targetLastSpotted = myBrain.currentTarget.position;
        myBrain.MyCharacterMove.SetRotation(targetLastSpotted - (Vector2)myBrain.transform.position);

        time += Time.deltaTime;
        if(time > 2f) { myBrain.ChangeStates(new EnemyGuard_Aggro()); return; }
    }
}

/// <summary>
/// How enemy guards process threats
/// </summary>
public class EnemyGuard_Aggro : BrainState {

    public Vector2 targetLastSpotted;

    public override void Enter(Brain brain) {
        base.Enter(brain);
        Debug.Log("Aggroed!");
    }

    public override void Execute() {
        targetLastSpotted = myBrain.currentTarget.position;
        myBrain.MyCharacterMove.SetRotation(targetLastSpotted - (Vector2)myBrain.transform.position);

        if (!myBrain.CheckVision(myBrain.currentTarget)) { // if we lose sight of the target
            myBrain.ChangeStates(new EnemyGuard_Chase()); // chase after them
        } else {
            myBrain.MainAction(); // attempt to attack
        }
    }

    public override void Exit() {
        int x = GameManager.GetGridSpaceX(targetLastSpotted.x);
        int y = GameManager.GetGridSpaceY(targetLastSpotted.y);
        myBrain.MyCharacterMove.SetDestination(x, y);
    }
}

/// <summary>
/// Previous state should initiate the SetDestination function
/// </summary>
public class EnemyGuard_Chase : BrainState {

    private bool searching = true; // assumes the calculate path coroutine is currently running

    public override void Execute() {
        base.Execute();
        if(myBrain.MyCharacterMove.movementRoutine == null) {
            if (searching) {
                searching = false; myBrain.MyCharacterMove.MoveToDestination(myBrain.MyCharacterMove.runSpeed);
            }
            else { myBrain.ChangeStates(new EnemyGuard_Scan()); }
        }
        if (!searching) {
            myBrain.MyCharacterMove.SetRotation(myBrain.MyCharacterMove.currentDestination - (Vector2)myBrain.transform.position);
        }
    }

    public override void Exit() {
        base.Exit();
        Debug.Log("Ahh!");
    }
}
