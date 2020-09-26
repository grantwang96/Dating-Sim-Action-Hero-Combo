using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour {

    [SerializeField] private Text _questTitle;
    [SerializeField] private Image _incompleteImage;
    [SerializeField] private Image _completedImage;
    [SerializeField] private Image _failedImage;
    [SerializeField] private RectTransform _objectivesParent;
    [SerializeField] private float _questFinishedHoldTime;

    [Tooltip("Text prefab for displaying quest objectives")]
    [SerializeField] private ObjectiveUI _questObjectivePrefab;

    private QuestState _currentQuestState;
    private readonly Queue<QuestState> _queuedQuestsToDisplay = new Queue<QuestState>();
    private readonly List<ObjectiveUI> _objectives = new List<ObjectiveUI>();

    public void SetQuestInfo(QuestState questState) {
        // if a quest is still currently being displayed, queue it
        if (_currentQuestState != null) {
            _queuedQuestsToDisplay.Enqueue(questState);
            return;
        }
        _currentQuestState = questState;
        if(_currentQuestState == null) {
            return;
        }
        if (IsQuestFinished(_currentQuestState.Status)) {
            AutoFinishQuest();
            return;
        }
        InitializeQuest();
    }

    private void InitializeQuest() {
        _currentQuestState.OnCompleted += OnQuestCompleted;
        _currentQuestState.OnFailed += OnQuestFailed;
        _currentQuestState.OnAbort += OnQuestAbort;

        GenerateObjectiveUI(_currentQuestState);
        UpdateObjectivesInfo(_currentQuestState);
        UpdateQuestInfo(_currentQuestState);
    }

    private static bool IsQuestFinished(QuestStatus status) {
        return status == QuestStatus.Aborted ||
            status == QuestStatus.Completed ||
            status == QuestStatus.Failed;
    }

    private void AutoFinishQuest() {
        switch (_currentQuestState.Status) {
            case QuestStatus.Completed:
                OnQuestCompleted();
                break;
            case QuestStatus.Failed:
                OnQuestFailed();
                break;
            case QuestStatus.Aborted:
                OnQuestAbort();
                break;
            default:
                break;
        }
    }

    private void OnQuestCompleted() {
        UnsubscribeFromQuest();
        if (enabled) {
            StartCoroutine(DisplayQuestFinishedState());
        }
    }

    private void OnQuestFailed() {
        UnsubscribeFromQuest();
        if (enabled) {
            StartCoroutine(DisplayQuestFinishedState());
        }
    }

    private void OnQuestAbort() {
        UnsubscribeFromQuest();
        if (enabled) {
            StartCoroutine(DisplayQuestFinishedState());
        }
    }

    private void UnsubscribeFromQuest() {
        _currentQuestState.OnCompleted -= OnQuestCompleted;
        _currentQuestState.OnFailed -= OnQuestFailed;
        _currentQuestState.OnAbort -= OnQuestAbort;
    }

    private void GenerateObjectiveUI(QuestState questState) {
        int diff = questState.ObjectiveStates.Count - _objectives.Count;
        for(int i = 0; i < diff; i++) {
            ObjectiveUI objectiveUI = Instantiate(_questObjectivePrefab, _objectivesParent);
            _objectives.Add(objectiveUI);
        }
    }

    private void UpdateObjectivesInfo(QuestState questState) {
        for(int i = 0; i < _objectives.Count; i++) {
            if(i >= questState.ObjectiveStates.Count) {
                _objectives[i].gameObject.SetActive(false);
                continue;
            }
            _objectives[i].gameObject.SetActive(true);
            _objectives[i].SetObjectiveInfo(questState.ObjectiveStates[i]);
        }
    }

    private void UpdateQuestInfo(QuestState questState) {
        _questTitle.text = questState.QuestName;
        _incompleteImage.enabled = questState.Status == QuestStatus.Ongoing;
        _completedImage.enabled = questState.Status == QuestStatus.Completed;
        _failedImage.enabled = questState.Status == QuestStatus.Failed;
    }

    private void HideAllObjectives() {
        for(int i = 0; i < _objectives.Count; i++) {
            _objectives[i].gameObject.SetActive(false);
        }
    }

    private IEnumerator DisplayQuestFinishedState() {
        yield return new WaitForSeconds(_questFinishedHoldTime);
        HideAllObjectives();
        LoadNextQuest();
    }

    private void LoadNextQuest() {
        _currentQuestState = null;
        if(_queuedQuestsToDisplay.Count > 0) {
            SetQuestInfo(_queuedQuestsToDisplay.Dequeue());
        }
    }
}
