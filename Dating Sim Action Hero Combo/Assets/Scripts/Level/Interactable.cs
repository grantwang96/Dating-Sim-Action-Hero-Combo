using System;

public interface IInteractable : ITileOccupant
{
    bool Interactable { get; }

    void InteractStart();
    void InteractHold();
    void InteractEnd();

    event Action OnCompleteInteraction;
}
