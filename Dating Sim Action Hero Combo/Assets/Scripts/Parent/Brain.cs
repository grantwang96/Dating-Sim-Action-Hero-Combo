using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// the parent class for AI entities
/// </summary>
public abstract class Brain : MonoBehaviour {

    protected BrainState currentState;

    [SerializeField] protected Damageable myDamageable;
    [SerializeField] protected CharacterMove myCharMove;
    public CharacterMove MyCharacterMove { get { return myCharMove; } }

    [SerializeField] protected List<Damageable> enemies = new List<Damageable>();
    public List<Damageable> Enemies { get { return enemies; } }
    public Damageable currentTarget;

    [SerializeField] protected LayerMask visionMask;
    [SerializeField] protected float rangeOfVision; // the maximum distance this character can see
    [SerializeField] protected float coneOfVision; // the maximum angle away this character can see

    // Use this for initialization
    protected virtual void Start() {
        myDamageable = GetComponent<Damageable>();
        myCharMove = GetComponent<CharacterMove>();
        if (currentState != null) { currentState.Enter(this); }
    }

    // Updat eis called once per frame
    protected virtual void Update() {
        if (currentState != null) { currentState.Execute(); }
    }

    public virtual Transform CheckVision() { // check the vision of the character and returns enemy transform if found
        foreach (Damageable enemy in enemies) {
            if (Vector2.Angle(enemy.transform.position - transform.position, transform.up) < coneOfVision &&
               Vector2.Distance(enemy.transform.position, transform.position) < rangeOfVision) {

                RaycastHit2D rayhit = Physics2D.Raycast(transform.position, enemy.transform.position - transform.position, rangeOfVision, visionMask);
                if (rayhit.transform == enemy.transform) { return enemy.transform; }
            }
        }
        return null;
    }

    public virtual bool CheckVision(Transform enemy) {
        if(enemy == null) { return false; }
        if (Vector2.Angle(enemy.position - transform.position, transform.up) < coneOfVision &&
               Vector2.Distance(enemy.position, transform.position) < rangeOfVision) {
            RaycastHit2D rayhit = Physics2D.Raycast(transform.position, enemy.position - transform.position, rangeOfVision, visionMask);
            if (rayhit.transform == enemy) { return true; }
        }
        return false;
    }

    public abstract void React(Damageable target);

    public abstract void MainAction();

    public virtual void ChangeStates(BrainState newState) {
        if(currentState != null) { currentState.Exit(); }
        currentState = newState;
        newState.Enter(this);
    }
}