using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerUnit : Unit {

    private const string PlayerHudId = "PlayerHud";

    public static PlayerUnit Instance { get; private set; }

    public static event Action OnPlayerUnitInstanceSet;

    [SerializeField] private PlayerOutfitController _playerOutfitController;

    private UIObject _playerHud;

    protected override void Awake() {
        Instance = this;
        OnPlayerUnitInstanceSet?.Invoke();
    }

    public override void Initialize(string unitId, UnitData unitData) {
        base.Initialize(unitId, unitData);
        _playerOutfitController.Initialize();
        UnitsManager.Instance.RegisterUnit(this);
        // temp
        UIObject newPlayerHud = UIManager.Instance.CreateNewUIObject(PlayerHudId, UILayerId.HUD);
        if(newPlayerHud != null) {
            _playerHud = newPlayerHud;
            _playerHud.Initialize();
            _playerHud.Display();
        }
        _playerOutfitController.OnOutfitChangeComplete += OnOutfitChanged;
        OnOutfitChanged();
    }

    public override void Dispose() {
        base.Dispose();
        _playerHud.Hide();
        UIManager.Instance.RemoveUIObject(PlayerHudId);
        _playerOutfitController.Dispose();
        _playerOutfitController.OnOutfitChangeComplete -= OnOutfitChanged;
    }

    private void OnOutfitChanged() {
        switch (_playerOutfitController.OutfitState) {
            case PlayerOutfitState.Agent:
                _unitTags = UnitTags.Agent;
                _detectableTags = DetectableTags.Agent;
                break;
            case PlayerOutfitState.Civilian:
                _unitTags = UnitTags.Civilian;
                _detectableTags = DetectableTags.Civilian;
                break;
            default:
                CustomLogger.Error(nameof(PlayerUnit), $"Could not update unit tags based on outfit state {_playerOutfitController.OutfitState}");
                break;
        }
    }
}
