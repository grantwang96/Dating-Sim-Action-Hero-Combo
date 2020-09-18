using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public interface IInputController {
    InputActionMap GameplayActionMap { get; }
    InputActionMap UIActionMap { get; }
}

public enum InputMapSet {
    Gameplay,
    UI
}

public class InputController : MonoBehaviour, IInputController
{
    public const string GameplayActionMapId = "Player";
    public const string UIActionMapId = "UI";

    public static IInputController Instance { get; private set; }
    
    public InputActionMap GameplayActionMap { get; private set; }
    public InputActionMap UIActionMap { get; private set; }

    [SerializeField] private InputActionAsset _gameplayInputAsset;

    private void Awake() {
        // internal class dependencies here
        Instance = this;
        GameplayActionMap = _gameplayInputAsset.FindActionMap(GameplayActionMapId);
        UIActionMap = _gameplayInputAsset.FindActionMap(UIActionMapId);

        GameEventsManager.StartGame?.Subscribe(OnGameStart);
        GameEventsManager.PauseMenu?.Subscribe(OnGamePaused);
        GameEventsManager.EndGame?.Subscribe(OnGameEnd);
    }

    private void OnDestroy() {
        GameEventsManager.StartGame?.Unsubscribe(OnGameStart);
        GameEventsManager.PauseMenu?.Unsubscribe(OnGamePaused);
        GameEventsManager.EndGame?.Unsubscribe(OnGameEnd);
    }

    private void SetGameplayInputsEnabled(bool enabled) {
        if (enabled) {
            GameplayActionMap.Enable();
        } else {
            GameplayActionMap.Disable();
        }
    }

    private void OnGameStart() {
        SetGameplayInputsEnabled(true);
    }

    private void OnGamePaused(bool paused) {
        SetGameplayInputsEnabled(!paused);
    }

    private void OnGameEnd(EndGameContext context) {
        SetGameplayInputsEnabled(false);
    }

    private void OnDialogQueued() {
        SetGameplayInputsEnabled(false);
    }

    private void OnDialogFinished() {
        SetGameplayInputsEnabled(true);
    }
}
