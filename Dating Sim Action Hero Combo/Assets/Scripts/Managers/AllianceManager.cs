using System;

public interface IAllianceManager : IInitializableManager
{
    event Action<NPCUnit, UnitMessage> OnAllianceMessageSent;
}