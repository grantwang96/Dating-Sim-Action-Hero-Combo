using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest Objective/Interact With Target")]
public class InteractWithTargetObjective : QuestObjectiveData
{
    [SerializeField] private List<InteractableObjectiveEntry> _interactableEntries = new List<InteractableObjectiveEntry>();
    public IReadOnlyList<InteractableObjectiveEntry> InteractableEntries => _interactableEntries;

    public override QuestObjectiveState CreateState() {
        return new InteractWithTargetObjectiveState(this);
    }
}

[System.Serializable]
public class InteractableObjectiveEntry {
    [SerializeField] private string _interactableId;
    [SerializeField] private string _interactablePrefabId;
    [SerializeField] private bool _requiresSpawn;
    [SerializeField] private IntVector3 _location;

    public string InteractableId => _interactableId;
    public string InteractablePrefabId => _interactablePrefabId;
    public bool RequiresSpawn => _requiresSpawn;
    public IntVector3 Location => _location;
}

public class InteractWithTargetObjectiveState : QuestObjectiveState
{
    private int _interactableCount;
    private List<IInteractable> _interactables = new List<IInteractable>();

    public InteractWithTargetObjectiveState(InteractWithTargetObjective data) : base(data) {
        _interactables.Clear();
        for(int i = 0; i < data.InteractableEntries.Count; i++) {
            RegisterInteractable(data.InteractableEntries[i]);
        }
        _interactableCount = _interactables.Count;
    }

    private void RegisterInteractable(InteractableObjectiveEntry entry) {
        if (entry.RequiresSpawn) {
            if(!PooledObjectManager.Instance.RegisterPooledObject(entry.InteractablePrefabId, 1)) {
                Debug.LogError($"[{nameof(InteractWithTargetObjectiveState)}]: Could not register interactable with prefab ID {entry.InteractablePrefabId}!");
                return;
            }
            if(!PooledObjectManager.Instance.UsePooledObject(entry.InteractablePrefabId, out PooledObject obj)) {
                Debug.LogError($"[{nameof(InteractWithTargetObjectiveState)}]: Failed to retrieve interactable with prefab ID {entry.InteractablePrefabId}!");
                return;
            }
            PooledInteractableInitData initData = new PooledInteractableInitData(entry.Location, entry.InteractableId);
            obj.Initialize(initData);
            obj.Spawn();
        }
        if(!LevelDataManager.Instance.TryGetInteractable(entry.InteractableId, out IInteractable interactable)) {
            Debug.LogError($"[{nameof(InteractWithTargetObjectiveState)}]: Could not find registered interactable with ID {entry.InteractablePrefabId}!");
            return;
        }
        interactable.OnCompleteInteraction += OnCompleteInteraction;
        _interactables.Add(interactable);
    }

    private void CleanUpInteractables() {
        for(int i = 0; i < _interactables.Count; i++) {
            _interactables[i].SetInteractable(false);
            PooledObject pooledObject = _interactables[i] as PooledObject;
            if(pooledObject != null) {
                pooledObject.Despawn();
                pooledObject.Dispose();
            }
        }
    }

    private void OnCompleteInteraction(IInteractable interactable) {
        interactable.OnCompleteInteraction -= OnCompleteInteraction;
        _interactableCount--;
        if(_interactableCount == 0) {
            OnAllInteractablesCompleted();
        }
        FireProgressUpdated();
    }

    private void OnAllInteractablesCompleted() {
        CleanUpInteractables();
        FireOnComplete();
    }
}
