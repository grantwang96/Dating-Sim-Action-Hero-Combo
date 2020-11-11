using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyInfo
{
    string Id { get; }
    string PatrolLoopId { get; }
    Unit Unit { get; }
}

public class EnemyInfo : IEnemyInfo
{
    public string Id { get; }
    public string PatrolLoopId { get; }
    public Unit Unit { get; }

    public EnemyInfo(string uniqueId, string patrolId, EnemyUnit unit) {
        Id = uniqueId;
        PatrolLoopId = patrolId;
        Unit = unit;
    }
}
