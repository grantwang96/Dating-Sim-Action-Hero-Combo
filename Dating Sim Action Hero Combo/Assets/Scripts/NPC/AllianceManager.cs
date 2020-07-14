using System;

public interface IAllianceManager
{
    event Action<NPCUnit, UnitMessage> OnAllianceMessageSent;
}