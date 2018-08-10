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
        ChangeStates(new GruntTravel());
    }

    // grunts will run away upon first seeing a threat. Afterwards defends against threat.
    public override void React(Transform target) {
        System.Type stateType = currentState.GetType();
        if (stateType == typeof(GruntTakeCover) || stateType == typeof(GruntDefend)) { return; }
        currentTarget = target;
        ChangeStates(new GruntTakeCover());
    }
}
