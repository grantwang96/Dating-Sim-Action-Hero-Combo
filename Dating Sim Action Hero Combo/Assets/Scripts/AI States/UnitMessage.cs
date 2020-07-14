using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// message that can be sent up to the Alliance manager for this Unit
public enum UnitMessage
{
    HostileFound,
    HostileLost,
    PlayerObjectiveInProgress,
    PlayerObjectiveCompleted,
    AlliedObjectiveInProgress,
    AlliedObjectiveCompleted
}
