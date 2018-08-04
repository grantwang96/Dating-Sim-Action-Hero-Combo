using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class of all AI state machines for NPCs
/// </summary>
public class BrainState {

    protected Brain myBrain;

    public virtual Vector3 Enter(Brain brain) { // set up the new brain state
        myBrain = brain; // save the brain
        return brain.transform.position;
    }

    public virtual void Execute() { // the update loop of the brain state
        
    }

    public virtual void Exit() { // what occurs when the state ends

    }
}
