using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitData : ScriptableObject
{
    [SerializeField] protected string _unitPrefabId;
    [SerializeField] protected int _maxHealth;
    [SerializeField] protected DamageType _resistances;
    [SerializeField] protected float _runSpeed;
    [SerializeField] protected float _walkSpeed;

    [SerializeField] protected float _visionRange;
    [SerializeField] protected float _visionAngle;
    [SerializeField] protected LayerMask _visionLayers;

    [SerializeField] protected int _wanderRadiusMin;
    [SerializeField] protected int _wanderRadiusMax;

    [SerializeField] protected UnitTags _hostileTags;

    [SerializeField] protected RuntimeAnimatorController _animatorController;

    public string UnitPrefabId => _unitPrefabId;
    public int MaxHealth => _maxHealth;
    public DamageType Resistances => _resistances;
    public float WalkSpeed => _walkSpeed;
    public float RunSpeed => _runSpeed;

    public float VisionRange => _visionRange;
    public float VisionAngle => _visionAngle;
    public LayerMask VisionLayers => _visionLayers;

    public int WanderRadiusMin => _wanderRadiusMin;
    public int WanderRadiusMax => _wanderRadiusMax;

    public UnitTags HostileTags => _hostileTags;

    public RuntimeAnimatorController AnimatorController => _animatorController;
}
