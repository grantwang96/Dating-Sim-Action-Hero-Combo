using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Stores data for setting up an enemy
/// </summary>
[CreateAssetMenu (menuName = "EnemyData/New Enemy Type")]
public class EnemyData : ScriptableObject {

    [SerializeField] private int _maxHealth; public int maxHealth { get { return _maxHealth; } }
    [SerializeField] private float _walkSpeed; public float walkSpeed { get { return _walkSpeed; } }
    [SerializeField] private float _runSpeed; public float runSpeed { get { return _runSpeed; } }
    [SerializeField] private WeaponType _startingWeapon; public WeaponType startingWeapon { get { return _startingWeapon; } }
}
