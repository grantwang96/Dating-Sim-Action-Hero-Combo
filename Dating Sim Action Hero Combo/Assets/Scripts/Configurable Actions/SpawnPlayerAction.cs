using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Configured Game Actions/Spawn Player")]
public class SpawnPlayerAction : ConfigurableGameAction
{
    [SerializeField] private PlayerConfig _playerConfig;

    public PlayerConfig PlayerUnitData => _playerConfig;

    public override IConfiguredGameActionState CreateActionState() {
        return new SpawnPlayerState(this);
    }
}

public class SpawnPlayerState : IConfiguredGameActionState {

    public event Action OnComplete;

    private PlayerConfig _playerConfig;

    public SpawnPlayerState(SpawnPlayerAction data) {
        _playerConfig = data.PlayerUnitData;
    }

    public void Execute() {
        if (!PooledObjectManager.Instance.UsePooledObject(_playerConfig.UnitData.UnitPrefabId, out PooledObject pooledObject)) {
            Debug.LogError($"[{nameof(SpawnPlayerAction)}]: Could not retrieve player prefab with id {_playerConfig.UnitData.UnitPrefabId}!");
            OnComplete?.Invoke();
            return;
        }
        PlayerUnit player = pooledObject as PlayerUnit;
        if(player == null) {
            Debug.LogError($"[{nameof(SpawnPlayerAction)}]: Pooled Object retrieved with id {_playerConfig.UnitData.UnitPrefabId} was not of type {nameof(PlayerUnit)}!");
            OnComplete?.Invoke();
            return;
        }
        PlayerInitializationData initData = new PlayerInitializationData() {
            OverrideUniqueId = _playerConfig.UnitData.UnitPrefabId,
            UnitData = _playerConfig.UnitData
        };
        player.Initialize(initData);
        player.Spawn();
        OnComplete?.Invoke();
    }
}
