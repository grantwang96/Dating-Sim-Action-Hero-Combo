using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit {

    public static PlayerUnit Instance { get; private set; }

    [SerializeField] private Transform _front;
    
    public override Transform Transform => transform;
    public override Transform Front => _front;
    public override IntVector3 MapPosition => LevelDataManager.Instance.WorldToArraySpace(transform.position);

    [SerializeField] private float _speed;

    private void Awake() {
        Instance = this;
    }
    
    public override void TakeDamage(int damage, DamageType damageType) {
        // visual response here
        base.TakeDamage(damage, damageType);
    }
}
