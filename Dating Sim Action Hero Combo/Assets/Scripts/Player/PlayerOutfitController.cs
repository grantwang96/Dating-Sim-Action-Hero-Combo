using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOutfitController : MonoBehaviour
{
    [SerializeField] private PlayerUnit _unit;
    [SerializeField] private bool _agentMode;
    private bool _outfitChangeInProgress;

    private IPlayerActionController _combatSet;
    private IPlayerActionController _civilianSet;
    private IPlayerActionController _currentSet;

    public bool AgentMode => _agentMode;
    
    // Start is called before the first frame update
    private void Start() {
        _civilianSet = new PlayerCivilianController(_unit);
        _combatSet = new PlayerCombatController(_unit);
        SetActionController(_agentMode);
        SubscribeToEvents();
    }

    private void SubscribeToEvents() {
        InputController.Instance.OutfitSwapBtnPressed += BeginOutfitChange;
    }

    private void UnsubscribeToEvents() {
        InputController.Instance.OutfitSwapBtnPressed -= BeginOutfitChange;
    }

    private void BeginOutfitChange() {
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
