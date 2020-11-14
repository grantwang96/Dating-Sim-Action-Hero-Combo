using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestInfoDisplay : UIObject {

    [SerializeField] private float _questTransitionTime;
    [SerializeField] private QuestUI _missionQuestUI;
    [SerializeField] private QuestUI _dateQuestUI;

    public override bool Initialize() {
        // subscribe to quest updates
        QuestManager.Instance.OnCurrentMissionQuestUpdated += OnMissionQuestUpdated;
        QuestManager.Instance.OnCurrentDateQuestUpdated += OnDateQuestUpdated;

        return base.Initialize();
    }

    public override void CleanUp() {
        Debug.Log("Removing quest info display...");
        Hide();
        QuestManager.Instance.OnCurrentMissionQuestUpdated -= OnMissionQuestUpdated;
        QuestManager.Instance.OnCurrentDateQuestUpdated -= OnDateQuestUpdated;
        _missionQuestUI.CleanUp();
        _dateQuestUI.CleanUp();
        base.CleanUp();
    }

    public override void Display() {
        base.Display();
        OnMissionQuestUpdated();
        OnDateQuestUpdated();
        gameObject.SetActive(true);
    }

    public override void Hide() {
        base.Hide();
        gameObject.SetActive(false);
    }

    private void OnMissionQuestUpdated() {
        // update the mission quest view with the new information
        _missionQuestUI.SetQuestInfo(QuestManager.Instance.CurrentMissionQuestState);
    }

    private void OnDateQuestUpdated() {
        // update the date quest view with the new information
        _dateQuestUI.SetQuestInfo(QuestManager.Instance.CurrentDateQuestState);
    }
}
