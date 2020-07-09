using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable : ITileOccupant
{
    bool Interactable { get; }

    void InteractStart();
    void InteractHold();
    void InteractEnd();
}
