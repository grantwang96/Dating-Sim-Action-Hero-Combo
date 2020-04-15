using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IInputController {

    Vector2 MovementInput { get; }
    Vector2 RotationInput { get; }

    event Action OutfitSwapBtnPressed;
    event Action OutfitSwapBtnHeld;
    event Action OutfitSwapBtnReleased;

    event Action InteractBtnPressed;
    event Action InteractBtnHeld;
    event Action InteractBtnReleased;

    event Action ShootBtnPressed;
    event Action ShootBtnHeld;
    event Action ShootBtnReleased;
}

public class InputController : MonoBehaviour, IInputController
{
    public static IInputController Instance { get; private set; }

    public Vector2 MovementInput { get; private set; }
    public Vector2 RotationInput { get; private set; }

    public event Action OutfitSwapBtnPressed;
    public event Action OutfitSwapBtnHeld;
    public event Action OutfitSwapBtnReleased;

    public event Action InteractBtnPressed;
    public event Action InteractBtnHeld;
    public event Action InteractBtnReleased;

    public event Action ShootBtnPressed;
    public event Action ShootBtnHeld;
    public event Action ShootBtnReleased;

    private void Awake() {
        // internal class dependencies here
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // out of class dependencies here
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInputs();
    }

    // TODO: split this into different control systems depending on last received input
    private void ProcessInputs() {
        KeyboardMouseInputs();
        // ControllerInputs();
        ButtonInputs();
    }

    private void KeyboardMouseInputs() {
        // get axis inputs for movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        MovementInput = new Vector2(moveHorizontal, moveVertical);

        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RotationInput = (mousePosition - (Vector2)PlayerUnit.Instance.Transform.position).normalized;
    }

    private void ControllerInputs() {
        // get axis inputs for rotation
        float rotationHorizontal = Input.GetAxis("RotHorizontal");
        float rotationVertical = Input.GetAxis("RotVertical");
        RotationInput = new Vector2(rotationHorizontal, rotationVertical);
    }

    private void ButtonInputs() {
        ButtonInput("Submit", InteractBtnPressed, InteractBtnHeld, InteractBtnReleased);
        ButtonInput("Fire1", ShootBtnPressed, ShootBtnHeld, ShootBtnReleased);
        ButtonInput("Interact", InteractBtnPressed, InteractBtnHeld, InteractBtnReleased);
        ButtonInput("SwitchOutfit", OutfitSwapBtnPressed, OutfitSwapBtnHeld, OutfitSwapBtnReleased);
    }

    private void ButtonInput(string inputName, Action pressed, Action held, Action released) {
        if (Input.GetButtonDown(inputName)) {
            pressed?.Invoke();
        } else if (Input.GetButton(inputName)) {
            held?.Invoke();
        } else if (Input.GetButtonUp(inputName)) {
            released?.Invoke();
        }
    }
}
