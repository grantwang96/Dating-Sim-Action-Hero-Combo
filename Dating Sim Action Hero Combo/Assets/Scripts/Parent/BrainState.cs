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
    }

    public virtual void Execute() { // the update loop of the brain state

        Transform target = myBrain.CheckVision(); // check vision for enemies
        if (target != null) {
            Damageable dam = target.GetComponent<Damageable>();
            if (dam && myBrain.Enemies.Contains(dam)) { myBrain.React(dam); }
        }
    }

    public virtual void Exit() { // what occurs when the state ends
        myBrain.MyCharacterMove.CancelDestination();
    }
}
