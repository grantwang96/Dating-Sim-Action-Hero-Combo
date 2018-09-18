using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateBrain : Brain {

    public static DateBrain Instance;
    public Transform dateLocation; // where the date should normally be
    
    [Range(-1f, 1f)] public float currentDateStanding;
    public float lastConversationTime; // save the last time the player has interacted with the date
    public Coroutine idleRoutine; // this coroutine runs while the date is satisfied
    
    private void Awake() {
        Instance = this;
    }

    protected override void Start() {
        ChangeStates(new Idle());
        base.Start();
    }

    /// <summary>
    /// This version checks for the player as well
    /// </summary>
    /// <returns></returns>
    public override Transform CheckVision() {
        foreach (Damageable enemy in GameManager.Instance.badGuys) {
            if (Vector2.Angle(enemy.transform.position - transform.position, transform.up) < myBluePrint.maxVisionAngle &&
               Vector2.Distance(enemy.transform.position, transform.position) < myBluePrint.rangeOfVision) {

                RaycastHit2D rayhit = Physics2D.Raycast(transform.position, enemy.transform.position - transform.position, myBluePrint.rangeOfVision, visionMask);
                if (rayhit.transform == enemy.transform) {
                    return enemy.transform;
                }
            }
        }
        if(Vector2.Angle(PlayerDamageable.Instance.transform.position - transform.position, transform.up) < myBluePrint.maxVisionAngle &&
           Vector2.Distance(PlayerDamageable.Instance.transform.position, transform.position) < myBluePrint.rangeOfVision
           && PlayerInput.Instance.agentModeOn) {
            return PlayerDamageable.Instance.transform;
        }
        return null;
    }
    
    /// <summary>
    /// If the player is the registered target, the game ends. Otherwise, react normally.
    /// </summary>
    /// <param name="target"></param>
    public override void ReactToThreat(Damageable target) {
        currentTarget = target;
        if(target == PlayerDamageable.Instance) { // the player has been seen in agent mode - Game Over
            GameManager.Instance.EndGame();
        } else {
            System.Type stateType = currentState.GetType();
            if (stateType == typeof(Threat_Detected)) { return; }
            currentTarget = target;
            ChangeStates(new Threat_Detected());
        }
    }

    public override void MainAction() {
        lastConversationTime = Time.time;
    }

    public override void Interact() {
        MainAction();
    }
}
