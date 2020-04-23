using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHud : UIObject
{
    [SerializeField] private FillBar _healthBar;

    [SerializeField] private int _currentHealth;
    [SerializeField] private int _maxHealth;

    private void Start() {
        PlayerStateController.Instance.OnHealthChanged += OnHealthChanged;
        _currentHealth = PlayerStateController.Instance.Health;
        _maxHealth = PlayerStateController.Instance.Health;
        _healthBar.UpdateValue((float)_currentHealth / _maxHealth);
    }

    private void OnDestroy() {
        PlayerStateController.Instance.OnHealthChanged -= OnHealthChanged;
    }

    private void OnHealthChanged(int change) {
        _currentHealth += change;
        _healthBar.UpdateValue((float)_currentHealth / _maxHealth);
    }

    public override void Display() {
        base.Display();
        gameObject.SetActive(true);
    }

    public override void Hide() {
        base.Hide();
        gameObject.SetActive(false);
    }
}
