using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : Damageable {

    public static PlayerDamageable Instance;

    [SerializeField] private int _armor;
    public int armor { get { return _armor; } }
    [SerializeField] private int _maxArmor;
    public int maxArmor { get { return _maxArmor; } }

    [SerializeField] private bool recovering;
    [SerializeField] private float recoverDelay;
    [SerializeField] private float recoverRate;

    private void Awake() {
        Instance = this;
    }

    protected override void Start() {
        // player is not registered with typical obstructions
    }

    private void Update() {
        if(armor < maxArmor && !recovering) { StartCoroutine(RecoverArmor()); }
    }

    private IEnumerator RecoverArmor() {
        recovering = true;
        _armor++;
        yield return new WaitForSeconds(recoverRate);
        recovering = false;
    }

    private IEnumerator RecoverArmorDelay() {
        recovering = true;
        yield return new WaitForSeconds(recoverDelay);
        recovering = false;
    }

    public override void TakeDamage(int damage, Vector2 sourcePoint) {
        if(armor > 0) {
            _armor -= damage;
            StopCoroutine(RecoverArmorDelay());
            StartCoroutine(RecoverArmorDelay());
        } else {
            _health -= damage;
        }
        
        if(_health <= 0) {
            Die();
        }
    }

    protected override void Die() {
        base.Die();
    }
}
