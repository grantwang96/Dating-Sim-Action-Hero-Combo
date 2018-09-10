using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "NPC/Enemy Grunt")]
public class EGrunt_Blueprint : Enemy_Blueprint {

    [SerializeField] protected int _coverRange;
    public int coverRange { get { return _coverRange; } }

    // enemy should be processing task if it has arrived at the current task location
    public override IEnumerator Idling(Brain brain) {
        
        while(EnemyTaskManager.Instance.currentTask.currentProgress < EnemyTaskManager.Instance.currentTask.timeToComplete) {
            Transform target = brain.CheckVision(); // check vision for enemies
            if (target != null) {
                Damageable dam = target.GetComponent<Damageable>();
                if (dam && brain.Enemies.Contains(dam)) {
                    brain.ReactToThreat(dam);
                    yield break;
                }
            }

            if (!EnemyTaskManager.Instance.currentTask.PerformAction(brain)) {
                brain.ChangeStates(new CalculatePath_Wander());
                yield break;
            }
            yield return null;
        }
        brain.ChangeStates(new CalculatePath_Wander());
    }

    // wandering is moving the grunt to the next task location
    public override void CalculatePathWander(Brain brain) {
        
        // obtain the location of the next task
        Vector2 location = EnemyTaskManager.Instance.currentTask.GetLocation();
        Debug.Log("Getting path location: " + location);

        // get the gridspace x, y
        // int locX = GameManager.GetGridSpaceX(location.x);
        // int locY = GameManager.GetGridSpaceY(location.y);
        int locX = Mathf.RoundToInt(location.x);
        int locY = Mathf.RoundToInt(location.y);

        // Set the destination of the brain
        Debug.Log("Setting destination...");
        brain.MyCharacterMove.SetDestination(locX, locY);
    }

    // the enemy should move towards the next task location
    public override void TravelExecute(Brain brain) {
        CheckForDanger(brain);

        if(brain.MyCharacterMove.movementRoutine == null) {
            brain.ChangeStates(new Idle());
        }
    }

    // the grunt should run for cover if it sees the player
    public override void ThreatDetectedEnter(Brain brain) {
        Vector2 threatDir = brain.transform.position - brain.currentTarget.transform.position;
        threatDir = threatDir.normalized;

        threatDir.x = Mathf.Round(threatDir.x);
        threatDir.y = Mathf.Round(threatDir.y);
        brain.MyCharacterMove.SetDestination(-threatDir, coverRange);
    }

    public override void ThreatDetectedExecute(Brain brain) {

        if (brain.CheckVision(brain.currentTarget.transform)) {
            Vector2 threatDir = brain.currentTarget.transform.position - brain.transform.position;
            brain.MyCharacterMove.SetRotation(threatDir);
        }

        if(brain.MyCharacterMove.movementRoutine == null) {
            if(brain.MyCharacterMove.pathSize > 0) { brain.MyCharacterMove.MoveToDestination(runSpeed, false); }
            else { AddressThreat(brain); }
        }
    }

    private void AddressThreat(Brain brain) {
        if (brain.CheckVision(brain.currentTarget.transform)) {
            brain.MainAction();
        }
    }
}
