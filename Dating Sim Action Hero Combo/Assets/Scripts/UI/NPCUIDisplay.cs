using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCUIDisplay : MonoBehaviour, PooledObject
{
    [SerializeField] private NPCUnit _unit;
    [SerializeField] private FillBar _healthBar;

    public NPCUnit Unit => _unit;

    public void Initialize(PooledObjectInitializationData initializationData) {
        NPCUIDisplayInitializationData initData = initializationData as NPCUIDisplayInitializationData;
        if(initData == null) {
            return;
        }
        _unit = initData.Unit;

        _unit.Damageable.OnCurrentHealthChanged += OnCurrentHealthChanged;
        _unit.Damageable.OnMaxHealthChanged += OnMaxHealthChanged;
    }

    private void FixedUpdate() {
        SetPosition();
    }

    public void Spawn() {
        gameObject.SetActive(true);
    }

    public void Despawn() {
        gameObject.SetActive(false);
    }
    
    private void SetPosition() {
        if (_unit != null) {
            transform.position = _unit.MoveController.Body.position;
        }
    }

    private void OnCurrentHealthChanged(int health) {
        _healthBar.UpdateValue((float)health / _unit.Damageable.MaxHealth);
    }

    private void OnMaxHealthChanged(int maxHealth) {
        _healthBar.UpdateValueInstant((float)_unit.Damageable.Health / maxHealth);
    }
}

public class NPCUIDisplayInitializationData : PooledObjectInitializationData {
    public NPCUnit Unit;
}
