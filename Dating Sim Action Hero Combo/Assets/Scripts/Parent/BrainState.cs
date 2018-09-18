using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class of all AI state machines for NPCs
/// </summary>
public class BrainState {

    protected Brain myBrain;

    public virtual void Enter(Brain brain) { // set up the new brain state
        myBrain = brain; // save the brain
        Debug.Log(brain.name + " has begun to " + this.GetType().Name);
        myBrain.MyBluePrint.IdleEnter(myBrain);
    }

    public virtual void Execute() { // the update loop of the brain state
        // myBrain.MyBluePrint.IdleExecute(myBrain);
    }

    public virtual void Exit() { // what occurs when the state ends
        // myBrain.MyCharacterMove.CancelDestination();
        // myBrain.MyBluePrint.IdleExit(myBrain);
    }
}

/// <summary>
/// Character is standing in one place, possibly looking around
/// </summary>
public class Idle : BrainState {

}

/// <summary>
/// Character is calculating a path within a certain radius
/// </summary>
public class CalculatePath_Wander : BrainState {

    public override void Enter(Brain brain) {
        Debug.Log(brain.name + " has begun to " + this.GetType().Name);
        myBrain = brain;
        myBrain.MyBluePrint.CalculatePathWander(brain);
    }
    
    public override void Execute() {
        myBrain.MyBluePrint.CalculatePathExec(myBrain);
    }

    public override void Exit() {
        myBrain.MyBluePrint.CalculateWanderExit(myBrain);
    }
}

/// <summary>
/// Character is calculating a path to a specific x and y position
/// </summary>
public class CalculatePath_Point : BrainState {

    public override void Enter(Brain brain) {
        myBrain = brain;
        myBrain.MyBluePrint.CalculatePathPoint(brain);
        Debug.Log(brain.name + " has begun to " + this.GetType().Name);
    }

    public override void Execute() {
        myBrain.MyBluePrint.CalculatePathExec(myBrain);
    }
}

/// <summary>
/// Character is calculating a path in a certain direction
/// </summary>
public class CalculatePath_Dir : BrainState {

    public override void Enter(Brain brain) {
        Debug.Log(brain.name + " has begun to " + this.GetType().Name);
        myBrain = brain;

        Vector2 threatDir = myBrain.currentTarget.transform.position - myBrain.transform.position;
        myBrain.MyBluePrint.CalculatePathDir(brain, threatDir);
    }

    public override void Execute() {
        myBrain.MyBluePrint.CalculatePathExec(myBrain);
    }
}

/// <summary>
/// Character is moving to a certain location
/// </summary>
public class Travel : BrainState {

    public override void Enter(Brain brain) {
        Debug.Log(brain.name + " has begun to " + this.GetType().Name);
        myBrain = brain;
        myBrain.MyBluePrint.TravelEnter(myBrain);
    }

    public override void Execute() {
        myBrain.MyBluePrint.TravelExecute(myBrain);
    }
}

/// <summary>
/// Character has detected a threat and must respond accordingly
/// </summary>
public class Threat_Detected : BrainState {

    public override void Enter(Brain brain) {
        Debug.Log(brain.name + " has begun to " + this.GetType().Name);
        myBrain = brain;
        brain.MyBluePrint.ThreatDetectedEnter(brain);
    }

    public override void Execute() {
        myBrain.MyBluePrint.ThreatDetectedExecute(myBrain);
    }
}

public interface Interactable {
    void Interact();
}