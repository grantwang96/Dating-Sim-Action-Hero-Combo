using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores the data and AI behaviours for the NPC
/// </summary>
public abstract class NPC_Blueprint : ScriptableObject {

    [SerializeField] protected int _maxHealth; // the starting health
    public int maxHealth { get { return _maxHealth; } }

    [SerializeField] protected float _walkSpeed; // the maximum walk speed
    public float walkSpeed { get { return _walkSpeed; } }
    [SerializeField] protected float _runSpeed; // the maximum run speed
    public float runSpeed { get { return _runSpeed; } }

    [SerializeField] protected float _rangeOfVision; // maximum vision distance
    public float rangeOfVision { get { return _rangeOfVision; } }
    [SerializeField] protected float _maxVisionAngle; // maximum cone of vision
    public float maxVisionAngle { get { return _maxVisionAngle; } }
    [SerializeField] protected float _scanSpeed; // the speed at which a character rotates to look around
    public float scanSpeed { get { return _scanSpeed; } }
    [SerializeField] protected float _scanRange;
    public float scanRange { get { return _scanRange; } }

    [SerializeField] protected int _maxWanderDistance;
    public int maxWanderDistance { get { return _maxWanderDistance; } }
    [SerializeField] protected float _idleTime;
    public float idleTime { get { return _idleTime; } }

    // functions that handle the idling state
    /// <summary>
    /// When the character begins idling
    /// </summary>
    /// <param name="brain"></param>
    public virtual void IdleEnter(Brain brain) {
        Debug.Log(brain + " has begun idling...");
        brain.StartCoroutine(Idling(brain));
    }
    /// <summary>
    /// The characters update loop
    /// </summary>
    /// <param name="brain"></param>
    public virtual void IdleExecute(Brain brain) {
        CheckForDanger(brain);
    }
    /// <summary>
    /// The characters idling coroutine. Accounts for idle time
    /// </summary>
    /// <param name="brain"></param>
    /// <returns></returns>
    public virtual IEnumerator Idling(Brain brain) {
        float time = 0f;
        while(time < idleTime) {
            Transform target = brain.CheckVision(); // check vision for enemies
            if (target != null) {
                Damageable dam = target.GetComponent<Damageable>();
                if (dam && brain.Enemies.Contains(dam)) {
                    brain.ReactToThreat(dam);
                    yield break;
                }
            }
            time += Time.deltaTime;
            yield return null;
        }
        brain.ChangeStates(new CalculatePath_Wander());
    }
    /// <summary>
    /// When the character stops idling
    /// </summary>
    /// <param name="brain"></param>
    public virtual void IdleExit(Brain brain) {
        brain.MyCharacterMove.CancelDestination();
    }

    // functions that handle calculating a path
    public virtual void CalculatePathWander(Brain brain) {
        Debug.Log(brain + " has begun calculating wander path...");
        brain.MyCharacterMove.SetDestination(_maxWanderDistance);
    }
    public virtual void CalculatePathPoint(Brain brain) {
        Debug.Log(brain + " has begun calculating path to " + brain.xDest + ", " + brain.yDest + "...");
        brain.MyCharacterMove.SetDestination(brain.xDest, brain.yDest);
    }
    public virtual void CalculatePathDir(Brain brain, Vector2 dir) {
        Debug.Log(brain + " has begun calculating path this way...");
        brain.MyCharacterMove.SetDestination(dir, _maxWanderDistance);
    }
    public virtual void CalculatePathExec(Brain brain) {
        CheckForDanger(brain);

        // if we have finished calculating a path
        if (brain.MyCharacterMove.movementRoutine == null) {
            brain.ChangeStates(new Travel());
        }
    }

    // functions that handle traveling along a path
    public virtual void TravelEnter(Brain brain) {
        Debug.Log(brain + " has begun traveling...");
        float moveSpeed = brain.alarmed ? runSpeed : walkSpeed;
        brain.MyCharacterMove.MoveToDestination(moveSpeed, true);
    }
    public virtual void TravelExecute(Brain brain) {
        CheckForDanger(brain);
        // Debug.Log(brain.MyCharacterMove.movementRoutine == null);

        // if we have finished moving to the destination
        if (brain.MyCharacterMove.movementRoutine == null) {
            brain.ChangeStates(new Idle());
        }
    }

    // functions that handle threats
    /// <summary>
    /// The default response will be the civilian's brain: run away
    /// </summary>
    /// <param name="brain"></param>
    public virtual void ThreatDetectedEnter(Brain brain) {
        
    }
    public virtual void ThreatDetectedExecute(Brain brain) {
        Vector2 dir = (brain.currentTarget.transform.position - brain.transform.position).normalized;
        dir.x = Mathf.Round(dir.x);
        dir.y = Mathf.Round(dir.y);
        brain.MyCharacterMove.MoveToDestination(dir, true, runSpeed);
    }

    protected void CheckForDanger(Brain brain) {
        Transform target = brain.CheckVision(); // check vision for enemies
        if (target != null) {
            Damageable dam = target.GetComponent<Damageable>();
            if (dam && brain.Enemies.Contains(dam)) { brain.ReactToThreat(dam); }
        }
    }
}
