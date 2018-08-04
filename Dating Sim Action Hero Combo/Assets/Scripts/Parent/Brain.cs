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
    [SerializeField] protected float rangeOfVision; // the maximum distance this character can see
    [SerializeField] protected float coneOfVision; // the maximum angle away this character can see

    // Use this for initialization
    protected virtual void Start() {
        myDamageable = GetComponent<Damageable>();
        myCharMove = GetComponent<CharacterMove>();
        if (currentState != null) { currentState.Enter(this); }
    }

    // Update is called once per frame
    protected virtual void Update() {
        if (currentState != null) { currentState.Execute(); }
        Transform target = CheckVision(); // check vision for enemies
        if(target != null) {
            Damageable dam = target.GetComponent<Damageable>();
            if (dam && enemies.Contains(dam)) { React((target.position - transform.position).normalized); }
        }
    }

    protected virtual Transform CheckVision() { // check the vision of the character and returns enemy transform if found
        foreach (Damageable enemy in enemies) {
            if (Vector2.Angle(enemy.transform.position - transform.position, transform.up) < coneOfVision &&
               Vector2.Distance(enemy.transform.position, transform.position) < rangeOfVision) {

                RaycastHit2D rayhit = Physics2D.Raycast(transform.position, enemy.transform.position - transform.position, rangeOfVision);
                if (rayhit.transform == enemy.transform) { return enemy.transform; }
            }
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, rangeOfVision);
        return hit.transform;
    }

    protected abstract void React(Vector2 dir);

    public virtual void ChangeStates(BrainState newState) {
        if(currentState != null) { currentState.Exit(); }
        currentState = newState;
        newState.Enter(this);
    }
}