using System;

public interface IInteractable : ITileOccupant
{
    bool Interactable { get; }
    void SetInteractable(bool interactable);

    void InteractStart();
    void InteractHold();
    void InteractEnd();

    event Action<IInteractable> OnCompleteInteraction;
}

public class PooledInteractableInitData : PooledObjectInitializationData {
    public readonly IntVector3 Position;
    public readonly string OverrideId;

    public PooledInteractableInitData(IntVector3 position, string overrideId = "") {
        Position = position;
        OverrideId = overrideId;
    }
}