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
    }
}
