using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageable : Damageable {

    public static PlayerDamageable Instance;

    [SerializeField] private int _armor;
    public int armor { get { return _armor; } }
    [SerializeField] private int _maxArmor;
    public int maxArmor { get { return _maxArmor; } }
    
    [SerializeField] private float recoverDelay;
    [SerializeField] private float recoverRate;

    private Coroutine armorRecoverRoutine;

    private void Awake() {
        Instance = this;
    }

    protected override void Start() {
        // player is not registered with typical obstructions
    }

    private void Update() {

    }

    private IEnumerator RecoverArmor() {
        while(armor < maxArmor) {
            _armor++;
            yield return new WaitForSeconds(recoverRate);
        }
        armorRecoverRoutine = null;
    }

    private IEnumerator RecoverArmorDelay() {
        yield return new WaitForSeconds(recoverDelay);
        armorRecoverRoutine = StartCoroutine(RecoverArmor());
    }

    public override void TakeDamage(int damage, Damageable source) {
        if(armor > 0) {
            _armor -= damage;
            if(armorRecoverRoutine != null) { StopCoroutine(armorRecoverRoutine); }
            armorRecoverRoutine = StartCoroutine(RecoverArmorDelay());
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
