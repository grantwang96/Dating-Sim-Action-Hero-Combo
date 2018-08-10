﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Brain state that moves grunt to current task location. Different from normal movement.
/// </summary>
public class GruntTravel : BrainState {

    private bool searching = true;

    public override void Enter(Brain brain) {
        base.Enter(brain);

        // obtain the location of the next task
        Vector2 location = EnemyTaskManager.Instance.currentTask.GetLocation();

        // get the gridspace x, y
        int locX = GameManager.GetGridSpaceX(location.x);
        int locY = GameManager.GetGridSpaceY(location.y);

        // Set the destination of the brain
        myBrain.MyCharacterMove.SetDestination(locX, locY);
    }

    public override void Execute() {
        base.Execute();

        if(myBrain.MyCharacterMove.movementRoutine == null) {
            if (searching) {
                searching = false;
                myBrain.MyCharacterMove.MoveToDestination(myBrain.MyCharacterMove.walkSpeed);
            } else {
                myBrain.ChangeStates(EnemyTaskManager.Instance.currentTask.PerformAction()); // perform current action
                return;
            }
        }

        if (!searching) {
            myBrain.MyCharacterMove.SetRotation(myBrain.MyCharacterMove.currentDestination - (Vector2)myBrain.transform.position);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}

public class GruntIdle : BrainState {

    float maxTime;
    float time = 0;

    public override void Enter(Brain brain) {
        base.Enter(brain);
        maxTime = Random.Range(2f, 5f);
    }

    public override void Execute() {
        base.Execute();

        time += Time.deltaTime;
        if(time >= maxTime) {
            myBrain.ChangeStates(new GruntWander());
        }
    }
}

public class GruntChat : BrainState {
    public override void Enter(Brain brain) {
        base.Enter(brain);
        
    }
}

public class GruntWander : BrainState {

    private bool searching = true;

    public override void Enter(Brain brain) {
        base.Enter(brain);

        Vector2 pos = EnemyTaskManager.Instance.currentTask.GetLocation();
        int x = Mathf.RoundToInt(pos.x);
        int y = Mathf.RoundToInt(pos.y);

        myBrain.MyCharacterMove.SetDestination(x, y);
    }

    public override void Execute() {
        base.Execute();

        if(myBrain.MyCharacterMove.movementRoutine == null) {
            if (searching) {
                searching = false;
                myBrain.MyCharacterMove.MoveToDestination(myBrain.MyCharacterMove.walkSpeed);
            } else {
                myBrain.ChangeStates(new GruntIdle());
            }
        }

        if (!searching) {
            myBrain.MyCharacterMove.SetRotation(myBrain.MyCharacterMove.currentDestination - (Vector2)myBrain.transform.position);
        }
    }

    public override void Exit() {
        base.Exit();
    }
}

public class GruntTakeCover : BrainState {

    private bool searching = true;

    public override void Enter(Brain brain) {
        base.Enter(brain);

        Vector2 threatDir = myBrain.currentTarget.position - myBrain.transform.position;
        threatDir = threatDir.normalized;

        threatDir.x = Mathf.Round(threatDir.x);
        threatDir.y = Mathf.Round(threatDir.y);
        Debug.Log(threatDir);

        myBrain.MyCharacterMove.SetDestination(threatDir);
    }

    public override void Execute() {
        if(myBrain.MyCharacterMove.movementRoutine == null) {
            if (searching) {
                searching = false;
                myBrain.MyCharacterMove.MoveToDestination(myBrain.MyCharacterMove.runSpeed);
            } else {
                myBrain.ChangeStates(new GruntDefend());
                return;
            }
        }
    }

    public override void Exit() {
        base.Exit();
    }
}

public class GruntDefend : BrainState {

    float time = 0f;
    float maxTime;

    private Vector2 targetLastSpotted;

    public override void Enter(Brain brain) {
        base.Enter(brain);
        maxTime = Random.Range(10f, 15f);
    }

    public override void Execute() {

        targetLastSpotted = myBrain.currentTarget.position;
        myBrain.MyCharacterMove.SetRotation(targetLastSpotted - (Vector2)myBrain.transform.position);
        if (!myBrain.CheckVision(myBrain.currentTarget)) {
            time += Time.deltaTime;
        } else {
            time = 0f;
            myBrain.MainAction(); // attempt to attack
        }

        if(time >= maxTime) { myBrain.ChangeStates(new GruntTravel()); }
    }
}