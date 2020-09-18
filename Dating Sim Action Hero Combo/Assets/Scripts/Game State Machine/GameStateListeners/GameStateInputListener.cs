using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class GameStateInputListener : GameStateListener
{
    [SerializeField] private string _inputId;
    [SerializeField] private InputMapSet _inputMapSet;

    protected override void OnGameStateEntered() {
        switch (_inputMapSet) {
            case InputMapSet.Gameplay:
                InputController.Instance.GameplayActionMap[_inputId].started += OnInputPressed;
                break;
            case InputMapSet.UI:
                InputController.Instance.UIActionMap[_inputId].started += OnInputPressed;
                break;
            default:
                break;
        }
    }

    protected override void OnGameStateExited() {
        switch (_inputMapSet) {
            case InputMapSet.Gameplay:
                InputController.Instance.GameplayActionMap[_inputId].started -= OnInputPressed;
                break;
            case InputMapSet.UI:
                InputController.Instance.UIActionMap[_inputId].started -= OnInputPressed;
                break;
            default:
                break;
        }
    }

    protected abstract void OnInputPressed(InputAction.CallbackContext context);
}
