using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest Objective/Interact With Target")]
public class InteractWithTargetObjective : QuestObjectiveData
{
    [SerializeField] private string _interactableId;
    public string InteractableId => _interactableId;

    public override QuestObjectiveState CreateState() {
        if (!LevelDataManager.Instance.TryGetInteractable(_interactableId, out IInteractable interactable)) {
            Debug.LogError($"[{name}]: Could not retrieve interactable with id {_interactableId}!");
            return null;
        }
        return new InteractWithTargetObjectiveState(this, interactable);
    }
}

public class InteractWithTargetObjectiveState : QuestObjectiveState
{
    private IInteractable _interactable;

    public InteractWithTargetObjectiveState(InteractWithTargetObjective data, IInteractable interactable) : base(data) {
        _interactable = interactable;
        _interactable.OnCompleteInteraction += OnCompleteInteraction;
    }

    private void OnCompleteInteraction() {
        _interactable.OnCompleteInteraction -= OnCompleteInteraction;
        FireOnComplete();
        FireProgressUpdated();
    }
}
