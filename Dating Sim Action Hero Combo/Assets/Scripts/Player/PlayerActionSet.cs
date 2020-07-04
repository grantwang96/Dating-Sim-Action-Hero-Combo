using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        InputController.Instance.InteractBtnPressed += OnInteractPressed;
        InputController.Instance.InteractBtnHeld += OnInteractHeld;
        InputController.Instance.InteractBtnReleased += OnInteractReleased;
    }

    protected virtual void UnsubscribeToEvents() {
        InputController.Instance.InteractBtnPressed -= OnInteractPressed;
        InputController.Instance.InteractBtnHeld -= OnInteractHeld;
        InputController.Instance.InteractBtnReleased -= OnInteractReleased;
    }

    protected virtual bool CanInteract() {
        if (_currentInteractable == null || !_currentInteractable.Interactable) {
            return false;
        }
        // allow interacting with objects
        return true;
    }

    private void OnInteractPressed() {
        if (!CanInteract()) {
            return;
        }
        // do interact things
        _currentInteractable.Interact();
    }

    private void OnInteractHeld() {
        if (!CanInteract()) {
            return;
        }
        // do interact things
    }

    private void OnInteractReleased() {
        if (!CanInteract()) {
            return;
        }
        // do interact things
    }

    private void OnUnitPositionUpdated(IntVector3 position) {
        /*
        _currentInteractable = null;
        List<ITileInfo> tiles = LevelDataManager.Instance.GetTilesWithinRadius(position, 1);
        // TODO: sort this list by priority if multiple objects are interactable. For now, just use first object
        for(int i = 0; i < tiles.Count; i++) {
            IInteractable interactable = (IInteractable) tiles[i].Occupant;
            if(interactable != null) {
                _currentInteractable = interactable;
                break;
            }
        }*/
    }
}
