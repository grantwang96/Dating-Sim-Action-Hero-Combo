using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class QuestInfoDisplay : UIObject {

    public event Action OnDisplayQuestCompleteFinished;
    public event Action OnDisplayQuestFailedFinished;
    public event Action OnDisplayQuestAbortedFinished;
    public bool QuestFinished { get; private set; } = true;

    [SerializeField] private Text _currentQuestText;

    private QuestState _questState;

    public override bool Initialize() {
        QuestFinished = false;
        if (IsQuestFinished(QuestManager.Instance.CurrentQuestState.Status)) {
            AutoFinishQuest(QuestManager.Instance.CurrentQuestState.Status);
            return base.Initialize();
        }
        _questState = QuestManager.Instance.CurrentQuestState;
        _currentQuestText.text = _questState.QuestDescription;
        return base.Initialize();
    }

    public override void CleanUp() {
        Hide();
        base.CleanUp();
    }

    private static bool IsQuestFinished(QuestStatus status) {
        return status == QuestStatus.Aborted || status == QuestStatus.Completed || status == QuestStatus.Failed;
    }

    private void AutoFinishQuest(QuestStatus status) {
        QuestFinished = true;
        switch (status) {
            case QuestStatus.Completed:
                OnQuestCompleted();
                break;
            case QuestStatus.Failed:
                OnQuestFailed();
                break;
            case QuestStatus.Aborted:
                break;
        }
    }

    private void OnQuestCompleted() {
        QuestFinished = true;
        // play some animation to show quest is completed.
        _currentQuestText.text = "Complete!";
        StartCoroutine(TempQuestFinishAnim(OnDisplayQuestCompleteFinished));
    }

    private void OnQuestFailed() {
        QuestFinished = true;
        _currentQuestText.text = "Failed!";
        StartCoroutine(TempQuestFinishAnim(OnDisplayQuestFailedFinished));
    }

    private void OnQuestAborted() {
        QuestFinished = true;
        _currentQuestText.text = "Aborted!";
        StartCoroutine(TempQuestFinishAnim(OnDisplayQuestFailedFinished));
    }

    private IEnumerator TempQuestFinishAnim(Action onFinish) {
        yield return new WaitForSeconds(3f);
        onFinish?.Invoke();
        Hide();
    }

    public override void Display() {
        base.Display();
        _questState.OnCompleted += OnQuestCompleted;
        _questState.OnFailed += OnQuestFailed;
        gameObject.SetActive(true);
    }

    public override void Hide() {
        base.Hide();
        gameObject.SetActive(false);
        if (_questState == null) {
            return;
        }
        _questState.OnCompleted -= OnQuestCompleted;
        _questState.OnFailed -= OnQuestFailed;
    }
}
