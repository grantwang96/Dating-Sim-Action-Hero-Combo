using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerOutfitController : MonoBehaviour, IUnitComponent
{
    private const string OutfitSwapInputId = "SwapOutfit";
    public static PlayerOutfitController Instance { get; private set; }

    public bool AgentMode => _agentMode;

    [SerializeField] private PlayerUnit _unit;
    [SerializeField] private bool _agentMode;
    private bool _outfitChangeInProgress;

    private IPlayerActionController _combatSet;
    private IPlayerActionController _civilianSet;
    private IPlayerActionController _currentSet;
    
    public void Initialize() {
        Instance = this;
        _civilianSet = new PlayerCivilianController(_unit);
        _combatSet = new PlayerCombatController(_unit);
        SetActionController(_agentMode);
        SubscribeToEvents();
    }

    public void Dispose() {
        _civilianSet.SetActive(false);
        _combatSet.SetActive(false);
        UnsubscribeToEvents();
    }
    
    private void SubscribeToEvents() {
        InputController.Instance.GameplayActionMap[OutfitSwapInputId].started += BeginOutfitChange;
    }

    private void UnsubscribeToEvents() {
        InputController.Instance.GameplayActionMap[OutfitSwapInputId].started -= BeginOutfitChange;
    }

    private void BeginOutfitChange(InputAction.CallbackContext context) {
        if (_outfitChangeInProgress) {
            return;
        }
        _outfitChangeInProgress = true;
        // do outfit change

        // temp just do it immediately for now
        FinishOutfitChange();
    }

    private void FinishOutfitChange() {
        _outfitChangeInProgress = false;
        _agentMode = !_agentMode;
        SetActionController(_agentMode);
    }

    private void SetActionController(bool agentMode) {
        if (agentMode) {
            _combatSet.SetActive(true);
            _civilianSet.SetActive(false);
            _currentSet = _combatSet;
        } else {
            _combatSet.SetActive(false);
            _civilianSet.SetActive(true);
            _currentSet = _civilianSet;
        }
    }
}
