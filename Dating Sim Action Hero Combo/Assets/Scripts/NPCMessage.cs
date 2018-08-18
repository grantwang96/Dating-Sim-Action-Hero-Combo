using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NPCMessage {

    protected Damageable dam;

    protected NPCMessage() {

    }
}

public class TravelMessage : NPCMessage {

    public Vector2 exitTrajectory; // the direction to leave

    TravelMessage(Vector2 newTraj, Damageable newDam) {
        exitTrajectory = newTraj;
        dam = newDam;
    }
}
