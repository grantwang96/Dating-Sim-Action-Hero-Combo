using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerUnit : Unit {

    private const string PlayerHudId = "PlayerHud";

    public static PlayerUnit Instance { get; private set; }

    [SerializeField] private PlayerOutfitController _playerOutfitController;

    private UIObject _playerHud;

    /*
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
    */

    public override void Dispose() {
        base.Dispose();
        _playerHud.Hide();
        UIManager.Instance.RemoveUIObject(PlayerHudId);
        _playerOutfitController.Dispose();
        _playerOutfitController.OnOutfitChangeComplete -= OnOutfitChanged;
    }

    public override void Initialize(PooledObjectInitializationData initializationData) {
        base.Initialize(initializationData);
        PlayerInitializationData initData = initializationData as PlayerInitializationData;
        if(initData == null) {
            Debug.LogError($"[{nameof(PlayerUnit)}]: Did not receive player initialization data!");
            return;
        }
        Instance = this;
        _playerOutfitController.Initialize();
        UnitsManager.Instance.RegisterUnit(this);
        _playerOutfitController.OnOutfitChangeComplete += OnOutfitChanged;
        OnOutfitChanged();
        Debug.Log($"[{nameof(PlayerUnit)}]: Initialized player character!");
    }

    public override void Spawn() {
        gameObject.SetActive(true);
    }

    public override void Despawn() {
        gameObject.SetActive(true);
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

public class PlayerInitializationData : UnitInitializationData {
    
}
