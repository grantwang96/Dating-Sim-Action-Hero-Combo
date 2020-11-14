using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public enum PlayerOutfitState {
    Unknown,
    Civilian,
    Agent
}

public class PlayerOutfitController : MonoBehaviour, IUnitComponent
{
    public static PlayerOutfitController Instance { get; private set; }

    public PlayerOutfitState OutfitState => _outfitState;
    public event Action<PlayerOutfitState> OnOutfitChangeStarted;
    public event Action OnOutfitChangeComplete;

    [SerializeField] private PlayerUnit _unit;
    [SerializeField] private PlayerOutfitState _outfitState;
    private bool _outfitChangeInProgress;

    private IPlayerActionController _combatSet;
    private IPlayerActionController _civilianSet;
    private IPlayerActionController _currentSet;
    
    public void Initialize() {
        Instance = this;
        _civilianSet = new PlayerCivilianController(_unit);
        _combatSet = new PlayerCombatController(_unit);
        GameEventsManager.StartGame.Subscribe(OnGameplayEntered);
        GameEventsManager.ExitGame.Subscribe(OnGameExited);
        GameEventsManager.PauseMenu.Subscribe(OnPauseMenu);
        GameEventsManager.EndGame.Subscribe(OnGameEnded);
        OnGameplayEntered();
    }

    public void Dispose() {
        OnGameplayExited();
        GameEventsManager.StartGame.Unsubscribe(OnGameplayEntered);
        GameEventsManager.ExitGame.Unsubscribe(OnGameExited);
        GameEventsManager.PauseMenu.Unsubscribe(OnPauseMenu);
        GameEventsManager.EndGame.Unsubscribe(OnGameEnded);
        OnOutfitChangeStarted = null;
        OnOutfitChangeComplete = null;
        Instance = null;
    }
    
    private void SubscribeToEvents() {
        InputController.Instance.GameplayActionMap[InputStrings.OutfitSwapKey].canceled += BeginOutfitChange;
    }

    private void UnsubscribeToEvents() {
        InputController.Instance.GameplayActionMap[InputStrings.OutfitSwapKey].canceled -= BeginOutfitChange;
    }

    private void BeginOutfitChange(InputAction.CallbackContext context) {
        if (_outfitChangeInProgress) {
            return;
        }
        _outfitChangeInProgress = true;
        _currentSet.SetActive(false);
        PlayerOutfitState newOutfitState;
        if (_outfitState == PlayerOutfitState.Civilian) {
            newOutfitState = PlayerOutfitState.Agent;
        } else {
            newOutfitState = PlayerOutfitState.Civilian;
        }
        OnOutfitChangeStarted?.Invoke(newOutfitState);
        // do outfit change
        StartCoroutine(ChangeOutfits());
    }

    private IEnumerator ChangeOutfits() {
        yield return new WaitForSeconds(3f);
        FinishOutfitChange();
    }
    
    private void FinishOutfitChange() {
        _outfitChangeInProgress = false;
        if(_outfitState == PlayerOutfitState.Civilian) {
            _outfitState = PlayerOutfitState.Agent;
        } else {
            _outfitState = PlayerOutfitState.Civilian;
        }
        SetActionController(_outfitState);
        OnOutfitChangeComplete?.Invoke();
    }

    private void SetActionController(PlayerOutfitState outfitMode) {
        _combatSet.SetActive(outfitMode == PlayerOutfitState.Agent);
        _civilianSet.SetActive(outfitMode == PlayerOutfitState.Civilian);
        switch (outfitMode) {
            case PlayerOutfitState.Agent:
                _currentSet = _combatSet;
                break;
            case PlayerOutfitState.Civilian:
                _currentSet = _civilianSet;
                break;
            default:
                break;
        }
    }

    private void OnPauseMenu(bool paused) {
        if (paused) {
            OnGameplayExited();
        } else {
            OnGameplayEntered();
        }
    }

    private void OnGameplayEntered() {
        SetActionController(_outfitState);
        SubscribeToEvents();
    }

    private void OnGameplayExited() {
        _civilianSet.SetActive(false);
        _combatSet.SetActive(false);
        UnsubscribeToEvents();
    }

    private void OnGameExited() {
        GameEventsManager.StartGame.Unsubscribe(OnGameplayEntered);
        GameEventsManager.ExitGame.Unsubscribe(OnGameExited);
        GameEventsManager.PauseMenu.Unsubscribe(OnPauseMenu);
        OnGameplayExited();
    }

    private void OnGameEnded(EndGameContext context) {
        GameEventsManager.StartGame.Unsubscribe(OnGameplayEntered);
        GameEventsManager.ExitGame.Unsubscribe(OnGameExited);
        GameEventsManager.PauseMenu.Unsubscribe(OnPauseMenu);
        OnGameplayExited();
    }
}
