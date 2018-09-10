using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// the parent class for AI entities
/// </summary>
public abstract class Brain : MonoBehaviour {

    protected BrainState currentState;

    [SerializeField] protected Damageable myDamageable;
    public int xPos { get { return myDamageable.XPos; } }
    public int yPos { get { return myDamageable.YPos; } }

    [SerializeField] protected CharacterMove myCharMove;
    public CharacterMove MyCharacterMove { get { return myCharMove; } }

    [SerializeField] protected NPC_Blueprint myBluePrint; // the blueprint of this NPC
    public NPC_Blueprint MyBluePrint { get { return myBluePrint; } }

    [SerializeField] protected List<Damageable> enemies = new List<Damageable>();
    public List<Damageable> Enemies { get { return enemies; } }
    public Damageable currentTarget;

    [SerializeField] protected LayerMask visionMask;
    /*
    [SerializeField] protected float rangeOfVision; // the maximum distance this character can see
    public float RangeOfVision { get { return rangeOfVision; } }
    [SerializeField] protected float coneOfVision; // the maximum angle away this character can see
    public float ConeOfVision { get { return coneOfVision; } }
    [SerializeField] protected float scanSpeed;
    public float ScanSpeed { get { return scanSpeed; } }

    [SerializeField] protected int _maxWanderDistance;
    public int maxWanderDistance { get { return _maxWanderDistance; } }
    */

    protected int _xDest;
    protected int _yDest;
    public int xDest { get { return _xDest; } }
    public int yDest { get { return _yDest; } }

    protected bool _alarmed = false;
    public bool alarmed { get { return _alarmed; } }

    // Use this for initialization
    protected virtual void Start() {
        myDamageable = GetComponent<Damageable>();
        myCharMove = GetComponent<CharacterMove>();
        if (currentState != null) { currentState.Enter(this); }
    }

    // Update is called once per frame
    protected virtual void Update() {
        if (currentState != null) { currentState.Execute(); }
    }

    public virtual Transform CheckVision() { // check the vision of the character and returns enemy transform if found
        foreach (Damageable enemy in enemies) {
            if (Vector2.Angle(enemy.transform.position - transform.position, transform.up) < myBluePrint.maxVisionAngle &&
               Vector2.Distance(enemy.transform.position, transform.position) < myBluePrint.rangeOfVision) {

                RaycastHit2D rayhit = Physics2D.Raycast(transform.position, enemy.transform.position - transform.position, myBluePrint.rangeOfVision, visionMask);
                if (rayhit.transform == enemy.transform) {
                    return enemy.transform;
                }
            }
        }
        return null;
    }

    public virtual bool CheckVision(Transform enemy) {
        if(enemy == null) { return false; }
        if (Vector2.Angle(enemy.position - transform.position, transform.up) < myBluePrint.maxVisionAngle &&
               Vector2.Distance(enemy.position, transform.position) < myBluePrint.rangeOfVision) {
            RaycastHit2D rayhit = Physics2D.Raycast(transform.position, enemy.position - transform.position, myBluePrint.rangeOfVision, visionMask);
            if (rayhit.transform == enemy) { return true; }
        }
        return false;
    }
    
    /// <summary>
    /// Set the current target to target, play any reaction animations, and change state to threat detected
    /// </summary>
    /// <param name="target"></param>
    public abstract void ReactToThreat(Damageable target);

    public virtual void OnThreatGone() {
        _alarmed = false;
    }

    public abstract void MainAction();

    public virtual void ChangeStates(BrainState newState) {
        if(currentState != null) { currentState.Exit(); }
        currentState = newState;
        newState.Enter(this);
    }
}