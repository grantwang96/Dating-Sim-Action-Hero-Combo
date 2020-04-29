using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCivilianController : PlayerActionController
{
    public static PlayerCivilianController Instance { get; private set; }

    public PlayerCivilianController(PlayerUnit unit) : base(unit) {
        Instance = this;
    }

    protected override bool CanInteract() {
        // allow interacting with both objects AND NPCs
        return base.CanInteract();
    }
}
