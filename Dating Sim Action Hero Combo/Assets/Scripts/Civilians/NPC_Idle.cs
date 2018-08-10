using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the idling state for normal civilian NPCs
/// </summary>
public class NPC_Idle : BrainState {

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
            myBrain.ChangeStates(new NPC_Wander());
        }
    }

    public override void Exit() {
        if(lookRoutine != null) {  }
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
