using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configured Game Actions/Despawn Player")]
public class DespawnPlayerAction : ConfigurableGameAction
{
    [SerializeField] private PlayerConfig _playerConfig;
    public PlayerConfig PlayerUnitData => _playerConfig;
    public override IConfiguredGameActionState CreateActionState() {
        return new DespawnPlayerActionState(this);
    }
}

public class DespawnPlayerActionState : IConfiguredGameActionState
{
    public event Action OnComplete;
    private PlayerConfig _playerConfig;

    public DespawnPlayerActionState(DespawnPlayerAction data) {
        _playerConfig = data.PlayerUnitData;
    }

    public void Execute() {
        if(PlayerUnit.Instance == null) {
            Debug.LogWarning($"[{nameof(DespawnPlayerAction)}]: Player Unit was null!");
            OnComplete?.Invoke();
            return;
        }
        string playerPrefabId = _playerConfig.UnitData.UnitPrefabId;
        PooledObjectManager.Instance.ReturnPooledObject(playerPrefabId, PlayerUnit.Instance);
    }
}
