using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectiveUI : MonoBehaviour
{
    [SerializeField] private Text _objectiveDescription;
    [SerializeField] private Image _completedImage;
    [SerializeField] private Image _incompleteImage;
    [SerializeField] private Image _failedImage;

    private QuestObjectiveState _questObjectiveState;

    public void SetObjectiveInfo(QuestObjectiveState objectiveState) {
        if(_questObjectiveState != null) {
            CleanUpObjectiveState();
        }
        _questObjectiveState = objectiveState;
        _questObjectiveState.OnProgressUpdated += OnObjectiveProgressUpdated;

        // update visuals to reflect objective state
        _objectiveDescription.text = objectiveState.ObjectiveDescription;
        OnObjectiveProgressUpdated(objectiveState);
    }

    private void CleanUpObjectiveState() {
        _questObjectiveState.OnProgressUpdated -= OnObjectiveProgressUpdated;
        _questObjectiveState = null;
    }

    private void OnObjectiveProgressUpdated(QuestObjectiveState objectiveState) {
        _objectiveDescription.text = objectiveState.ObjectiveDescription;
        _incompleteImage.enabled = objectiveState.Status == QuestObjectiveStatus.Ongoing;
        _completedImage.enabled = objectiveState.Status == QuestObjectiveStatus.Completed;
        _failedImage.enabled = objectiveState.Status == QuestObjectiveStatus.Failed;
    }
}
