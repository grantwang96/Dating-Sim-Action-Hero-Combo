using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the idling state for normal civilian NPCs
/// </summary>
public class Civilian_Idle : BrainState {

    private Coroutine lookRoutine;
    private float speed = .5f;
    private float range = 60f;

    public override void Enter(Brain brain) {
        myBrain = brain;
        lookRoutine = myBrain.StartCoroutine(lookAround(brain.transform.eulerAngles.z));
    }

    public override void Execute() {
        base.Execute();
        if(lookRoutine == null) {
            myBrain.ChangeStates(new Civilian_Wander());
        }
    }

    public override void Exit() {
        if(lookRoutine != null) { myBrain.StopCoroutine(lookRoutine); }
    }

    private IEnumerator lookAround(float startingAngle) {
        float time = 0f;
        while(time < 1f) {
            myBrain.transform.eulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(startingAngle, startingAngle + range, time));
            time += Time.deltaTime * speed;
            yield return null;
        }
        time = 0f;
        while(time < 1f) {
            myBrain.transform.eulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(startingAngle + range, startingAngle - range, time));
            time += Time.deltaTime * speed / 2;
            yield return null;
        }
        time = 0f;
        while(time < 1f) {
            myBrain.transform.eulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(startingAngle - 60f, startingAngle, time));
            time += Time.deltaTime * speed;
            yield return null;
        }
        lookRoutine = null;
    }
}

/// <summary>
/// This is the wandering state of normal civilian NPCs
/// </summary>
public class Civilian_Wander : BrainState {

    private bool searchingForPath = true;

    public override void Enter(Brain brain) {
        base.Enter(brain);
        myBrain.MyCharacterMove.SetDestination(8);
    }

    public override void Execute() {
        if (myBrain.MyCharacterMove.movementRoutine == null) {
            if (searchingForPath) {
                searchingForPath = false;
                myBrain.MyCharacterMove.MoveToDestination(myBrain.MyBluePrint.walkSpeed, true);
            } else {
                myBrain.ChangeStates(new Civilian_Idle());
            }
        }
        if (!searchingForPath) {
            myBrain.MyCharacterMove.SetRotation(myBrain.MyCharacterMove.currentDestination - (Vector2)myBrain.transform.position);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}

/// <summary>
/// This has the civilian move to a specific location
/// </summary>
public class Civilian_Travel : BrainState {

    private bool searchingForPath = true;
    int newX;
    int newY;

    Civilian_Travel(int xPos, int yPos) {
        newX = xPos;
        newY = yPos;
    }

    public override void Enter(Brain brain) {
        base.Enter(brain);
    }

    public override void Execute() {
        base.Execute();

        if (myBrain.MyCharacterMove.movementRoutine == null) {
            if (searchingForPath) {
                searchingForPath = false;
                myBrain.MyCharacterMove.MoveToDestination(myBrain.MyBluePrint.walkSpeed, true);
            } else {
                myBrain.ChangeStates(new Civilian_Idle());
            }
        }
        if (!searchingForPath) {
            myBrain.MyCharacterMove.SetRotation(myBrain.MyCharacterMove.currentDestination - (Vector2)myBrain.transform.position);
        }
    }
}

/// <summary>
/// During this state the civilian will calculate a route of escape and then switch to runaway
/// </summary>
public class Civilian_Panic : BrainState {

    public override void Enter(Brain brain) {
        base.Enter(brain);

        Vector2 threatDir = myBrain.transform.position - myBrain.currentTarget.transform.position;
        threatDir = threatDir.normalized;

        threatDir.x = Mathf.Round(threatDir.x);
        threatDir.y = Mathf.Round(threatDir.y);

        myBrain.MyCharacterMove.MoveToDestination(threatDir, true, myBrain.MyBluePrint.runSpeed);
        // myBrain.MyCharacterMove.SetDestination(threatDir, Mathf.RoundToInt(myBrain.RangeOfVision));
    }

    public override void Execute() {
        
        if(myBrain.MyCharacterMove.movementRoutine == null) {
            myBrain.ChangeStates(new Civilian_RunAway());
        }
    }

    public override void Exit() {

    }
}

/// <summary>
/// When Civilians run away from threats
/// </summary>
public class Civilian_RunAway : BrainState {
    private bool searching = true;

    public override void Enter(Brain brain) {
        base.Enter(brain);
        Debug.Log("Run Away!");
        Vector2 threatDir = myBrain.transform.position - myBrain.currentTarget.transform.position;
        threatDir = threatDir.normalized;

        threatDir.x = Mathf.Round(threatDir.x);
        threatDir.y = Mathf.Round(threatDir.y);

        myBrain.MyCharacterMove.MoveToDestination(threatDir, true, myBrain.MyBluePrint.runSpeed);
        // myBrain.MyCharacterMove.MoveToDestination(myBrain.MyCharacterMove.runSpeed, true);
    }

    public override void Execute() {
        
        if(myBrain.MyCharacterMove.movementRoutine == null) {
            if (myBrain.CheckVision(myBrain.currentTarget.transform)) {
                myBrain.ChangeStates(new Civilian_RunAway());
                return;
            } else {
                myBrain.ChangeStates(new Civilian_Hide());
            }
        }
    }

    public override void Exit() {
        myBrain.MyCharacterMove.CancelDestination();
    }
}

/// <summary>
/// When the civilian hides from threats
/// </summary>
public class Civilian_Hide : BrainState {

    float time = 0f;
    float maxTime = 20f;

    float speed = 1f;
    float range = 60f;
    Coroutine lookRoutine;

    public override void Enter(Brain brain) {
        base.Enter(brain);
        Damageable dam = myBrain.GetComponent<Damageable>();
        SetDirection(dam.XPos, dam.YPos);

        myBrain.StartCoroutine(lookAround(dam.transform.eulerAngles.z));
    }

    public override void Execute() {
        base.Execute();
        time += Time.deltaTime;

        if(time >= maxTime) {
            myBrain.ChangeStates(new Civilian_Idle());
        }

        if(lookRoutine == null) {
            lookRoutine = myBrain.StartCoroutine(lookAround(myBrain.transform.eulerAngles.z));
        }
    }

    // pass a position in grid space and sets the civilians rotation to face away from a wall
    private void SetDirection(int x, int y) {
        Vector2[] dirs = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        
        for(int i = 0; i < dirs.Length; i++) {
            Vector2 temp = dirs[i];
            int rand = Random.Range(0, dirs.Length);
            dirs[i] = dirs[rand];
            dirs[rand] = temp;
        }

        for(int i = 0; i < dirs.Length; i++) {
            int newX = Mathf.RoundToInt(dirs[i].x);
            int newY = Mathf.RoundToInt(dirs[i].y);
            
            if(GameManager.Instance.IsWithinGridSpace(newX + x, newY + y) && GameManager.Instance.grid[newX + x, newY + y] == null) { // check space to see if empty
                myBrain.MyCharacterMove.SetRotation(dirs[i]);
                return;
            }
        }
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