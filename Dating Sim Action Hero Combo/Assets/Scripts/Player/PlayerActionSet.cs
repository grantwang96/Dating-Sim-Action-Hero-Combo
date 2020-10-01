using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public interface IPlayerActionController
{
    void SetActive(bool active);
}

public class PlayerActionController : IPlayerActionController {
    
    protected PlayerUnit _unit;

    protected IInteractable _currentInteractable;
    public IInteractable CurrentInteractable => _currentInteractable;

    public PlayerActionController(PlayerUnit unit) {
        _unit = unit;
        _unit.MoveController.OnMapPositionUpdated += OnUnitPositionUpdated;
    }

    public virtual void SetActive(bool active) {
        if (active) {
            SubscribeToEvents();
        } else {
            UnsubscribeToEvents();
        }
        OnUnitPositionUpdated(_unit.MoveController.MapPosition);
    }

    protected virtual void SubscribeToEvents() {
        InputController.Instance.GameplayActionMap[InputStrings.InteractKey].started += OnInteractPressed;
        InputController.Instance.GameplayActionMap[InputStrings.InteractKey].performed += OnInteractHeld;
        InputController.Instance.GameplayActionMap[InputStrings.InteractKey].canceled += OnInteractReleased;
    }

    protected virtual void UnsubscribeToEvents() {
        InputController.Instance.GameplayActionMap[InputStrings.InteractKey].started -= OnInteractPressed;
        InputController.Instance.GameplayActionMap[InputStrings.InteractKey].performed -= OnInteractHeld;
        InputController.Instance.GameplayActionMap[InputStrings.InteractKey].canceled -= OnInteractReleased;
    }

    protected virtual bool CanInteract() {
        if (_currentInteractable == null || !_currentInteractable.Interactable) {
            return false;
        }
        // allow interacting with objects
        return true;
    }

    private void OnInteractPressed(InputAction.CallbackContext context) {
        if (!CanInteract()) {
            return;
        }
        // do interact things
        _currentInteractable.InteractStart();
    }

    private void OnInteractHeld(InputAction.CallbackContext context) {
        if (!CanInteract()) {
            return;
        }
        MonoBehaviourMaster.Instance.OnUpdate += ProcessHeldInteract;
    }

    private void ProcessHeldInteract() {
        // do interact things
        _currentInteractable.InteractHold();
    }

    private void OnInteractReleased(InputAction.CallbackContext context) {
        if (!CanInteract()) {
            return;
        }
        // do interact things
        _currentInteractable.InteractEnd();
        MonoBehaviourMaster.Instance.OnUpdate -= ProcessHeldInteract;
    }

    private void OnUnitPositionUpdated(IntVector3 position) {
        _currentInteractable = null;
        List<ITileInfo> tiles = LevelDataManager.Instance.GetTilesWithinRadius(position, 1);
        for(int i = 0; i < tiles.Count; i++) {
            if(tiles[i].Occupants.Count == 0) {
                continue;
            }
            // TODO: sort this list by priority if multiple objects are interactable. For now, just use first object
            for (int j = 0; j < tiles[i].Occupants.Count; j++) {
                IInteractable interactable = tiles[i].Occupants[i] as IInteractable;
                if (interactable != null) {
                    _currentInteractable = interactable;
                    break;
                }
            }
        }
    }
}
