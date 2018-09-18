using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grunts will attempt to finish the mission tasks
/// </summary>
public class EnemyBrain_Grunt : EnemyBrain {

    protected override void Start() {

        if (!enemies.Contains(PlayerDamageable.Instance)) { enemies.Add(PlayerDamageable.Instance); }
        myDamageable = GetComponent<Damageable>();
        myCharMove = GetComponent<CharacterMove>();

        _currentClip = heldWeapon.clipCapacity;
        ChangeStates(new Idle());
    }

    // grunts will run away upon first seeing a threat. Afterwards defends against threat.
    public override void ReactToThreat(Damageable target) {
        System.Type stateType = currentState.GetType();
        if (stateType == typeof(CalculatePath_Dir) || stateType == typeof(Threat_Detected)) { return; }
        currentTarget = target;
        ChangeStates(new Threat_Detected());
    }
}
