using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This is the wandering state of normal civilian NPCs
/// </summary>
public class NPC_Wander : BrainState {

    private bool searchingForPath = true;

    public override Vector3 Enter(Brain brain) {

        brain.MyCharacterMove.SetDestination(4);
        return base.Enter(brain);
    }

    public override void Execute() {
        if (myBrain.MyCharacterMove.movementRoutine == null) {
            if (searchingForPath) {
                searchingForPath = false;
                myBrain.MyCharacterMove.MoveToDestination();
            } else {
                myBrain.ChangeStates(new NPC_Idle());
            }
        }
    }

    public override void Exit() {
        base.Exit();
    }
}
