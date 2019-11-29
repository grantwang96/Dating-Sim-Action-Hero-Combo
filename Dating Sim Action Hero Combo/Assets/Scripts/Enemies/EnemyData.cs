using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    [SerializeField] private string _unitPrefabId;
    [SerializeField] private int _maxHealth;
    [SerializeField] private float _runSpeed;
    [SerializeField] private float _walkSpeed;

    [SerializeField] private float _visionRange;
    [SerializeField] private float _visionAngle;
    [SerializeField] private LayerMask _visionLayers;

    [SerializeField] private int _wanderRadiusMin;
    [SerializeField] private int _wanderRadiusMax;

    [SerializeField] private RuntimeAnimatorController _animatorController;

    [SerializeField] private List<AIStateTransitionEntry> _aiStateMachine = new List<AIStateTransitionEntry>();

    public string UnitPrefabId => _unitPrefabId;
    public int MaxHealth => _maxHealth;
    public float WalkSpeed => _walkSpeed;
    public float RunSpeed => _runSpeed;

    public float VisionRange => _visionRange;
    public float VisionAngle => _visionAngle;
    public LayerMask VisionLayers => _visionLayers;

    public int WanderRadiusMin => _wanderRadiusMin;
    public int WanderRadiusMax => _wanderRadiusMax;

    public RuntimeAnimatorController AnimatorController => _animatorController;

    public IReadOnlyList<AIStateTransitionEntry> AIStateMachine => _aiStateMachine;

    public List<AIStateDataObject> GetStateForTransitionId(AIStateTransitionId transitionId) {
        for(int i = 0; i < _aiStateMachine.Count; i++) {
            if(_aiStateMachine[i].TransitionId == transitionId) {
                return _aiStateMachine[i].States;
            }
        }
        return new List<AIStateDataObject>();
    }
}
