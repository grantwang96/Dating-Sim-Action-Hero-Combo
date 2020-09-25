using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public interface IInputController {
    PlayerInput PlayerInput { get; }
    InputActionMap GameplayActionMap { get; }
    InputActionMap UIActionMap { get; }

    void SetGameplayInputsEnabled(bool enabled);
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
    
    public PlayerInput PlayerInput { get; private set; }
    public InputActionMap GameplayActionMap { get; private set; }
    public InputActionMap UIActionMap { get; private set; }

    [SerializeField] private InputActionAsset _gameplayInputAsset;

    private void Awake() {
        // internal class dependencies here
        Instance = this;
        PlayerInput = GetComponent<PlayerInput>();
        GameplayActionMap = _gameplayInputAsset.FindActionMap(GameplayActionMapId);
        UIActionMap = _gameplayInputAsset.FindActionMap(UIActionMapId);
        PlayerInput.onControlsChanged += OnControlsChanged;

        GameEventsManager.StartGame.Subscribe(OnGameStart);
        GameEventsManager.EndGame.Subscribe(OnGameEnd);
        GameEventsManager.ExitGame.Subscribe(OnGameExit);
        GameEventsManager.PauseMenu.Subscribe(OnGamePaused);

        DialogManager.Instance.OnDialogQueued += OnDialogQueued;
        DialogManager.Instance.OnShowDialogFinished += OnShowDialogFinished;
    }

    private void OnDestroy() {
        PlayerInput.onControlsChanged -= OnControlsChanged;

        DialogManager.Instance.OnDialogQueued -= OnDialogQueued;
        DialogManager.Instance.OnShowDialogFinished -= OnShowDialogFinished;
    }

    private void OnControlsChanged(PlayerInput playerInput) {

    }

    private void OnGameStart() {
        SetGameplayInputsEnabled(true);
    }

    private void OnGameEnd(EndGameContext context) {
        SetGameplayInputsEnabled(false);
    }

    private void OnGameExit() {
        SetGameplayInputsEnabled(false);
    }

    private void OnGamePaused(bool gamePaused) {
        SetGameplayInputsEnabled(!gamePaused);
    }

    private void OnDialogQueued() {
        SetGameplayInputsEnabled(false);
    }

    private void OnShowDialogFinished() {
        SetGameplayInputsEnabled(true);
    }

    public void SetGameplayInputsEnabled(bool enabled) {
        if (enabled) {
            GameplayActionMap.Enable();
        } else {
            GameplayActionMap.Disable();
        }
    }
}
