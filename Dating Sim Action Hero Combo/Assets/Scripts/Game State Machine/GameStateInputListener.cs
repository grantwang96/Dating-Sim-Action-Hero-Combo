using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class GameStateInputListener : MonoBehaviour
{
    [SerializeField] private string _inputId;
    [SerializeField] private GameState _gameState;
    [SerializeField] private InputMapSet _inputMapSet;

    private void Awake() {
        _gameState.OnGameStateEnter += OnGameStateEnter;
        _gameState.OnGameStateExit += OnGameStateExit;
    }

    private void OnDestroy() {
        _gameState.OnGameStateEnter -= OnGameStateEnter;
        _gameState.OnGameStateExit -= OnGameStateExit;
    }

    private void OnGameStateEnter() {
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

    private void OnGameStateExit() {
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
