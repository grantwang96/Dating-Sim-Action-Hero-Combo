using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCivilianController : PlayerActionController
{
    public PlayerCivilianController(PlayerUnit unit) : base(unit) { }

    protected override bool CanInteract() {
        // allow interacting with both objects AND NPCs
        return base.CanInteract();
    }
}
