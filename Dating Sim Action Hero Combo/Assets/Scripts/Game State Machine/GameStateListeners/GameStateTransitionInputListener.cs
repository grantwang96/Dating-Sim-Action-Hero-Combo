using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateTransitionInputListener : GameStateInputListener
{
    [SerializeField] private string _transitionId;

    protected override void OnInputPressed(InputAction.CallbackContext context) {
        GameStateManager.Instance.HandleTransition(_transitionId);
    }
}
