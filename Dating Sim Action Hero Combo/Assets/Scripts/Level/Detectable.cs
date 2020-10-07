using UnityEngine;

public interface IDetectable
{
    Transform Transform { get; }
    DetectableTags DetectableTags { get; }
}

[System.Flags]
public enum DetectableTags {
    Player = (1 << 0),
    Civilian = (1 << 1),
    Law_Enforcement = (1 << 2),
    Enemy = (1 << 3),
    Date = (1 << 4),
    Agent = (1 << 5)
}
