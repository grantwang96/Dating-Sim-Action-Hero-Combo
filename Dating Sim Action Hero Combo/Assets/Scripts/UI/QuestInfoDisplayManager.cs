using System;
using System.Collections.Generic;

public class QuestInfoDisplayManager : IInitializableManager {

    private const string QuestInfoDisplayId = "QuestInfoDisplay";

    private QuestInfoDisplay _currentQuestInfoDisplay; // the current objective for the 
    private Queue<QuestState> _queuedQuestStates = new Queue<QuestState>(); // queue of quest states to display statuses for

    public void Initialize(Action<bool> initializationCallback = null) {
        // generate the current quest info display
        UIObject uiObject = UIManager.Instance.CreateNewUIObject(QuestInfoDisplayId, UILayerId.Notifications);
        _currentQuestInfoDisplay = uiObject as QuestInfoDisplay;
        if (_currentQuestInfoDisplay == null) {
            CustomLogger.Error(nameof(QuestInfoDisplayManager), $"Could not create current quest info display with id: {QuestInfoDisplayId}");
            initializationCallback?.Invoke(false);
            return;
        }
        _currentQuestInfoDisplay.Hide();
        // listen to quest info display events
        _currentQuestInfoDisplay.OnDisplayQuestCompleteFinished += OnQuestDisplayCompleteFinished;
        _currentQuestInfoDisplay.OnDisplayQuestFailedFinished += OnQuestDisplayFailedFinished;

        // listen to quest manager events
        QuestManager.Instance.OnCurrentQuestUpdated += OnCurrentQuestUpdated;
        QuestManager.Instance.OnAllQuestsCompleted += OnAllQuestsCompleted;

        CustomLogger.Log(nameof(QuestManager), $"Initializing {nameof(QuestInfoDisplayManager)}");
        initializationCallback?.Invoke(true);
    }

    // when a new quest has been assigned
    private void OnCurrentQuestUpdated() {
        if(_currentQuestInfoDisplay == null) {
            CustomLogger.Error(nameof(QuestInfoDisplayManager), $"CurrentQuestInfoDisplay is null!");
            return;
        }
        if(_queuedQuestStates.Count != 0 || !_currentQuestInfoDisplay.QuestFinished) {
            // queue up the next quest state to display after this one has finished
            _queuedQuestStates.Enqueue(QuestManager.Instance.CurrentQuestState);
        } else {
            // initialize and re-display the info display
            _currentQuestInfoDisplay.Initialize();
            _currentQuestInfoDisplay.Display();
        }
    }

    private void OnQuestDisplayCompleteFinished() {
        if(_queuedQuestStates.Count == 0) {
            return;
        }
        // get the next quest to display
        QuestState nextQuest = _queuedQuestStates.Dequeue();
        _currentQuestInfoDisplay.Initialize();
        _currentQuestInfoDisplay.Display();
    }

    private void OnQuestDisplayFailedFinished() {
        if (_queuedQuestStates.Count == 0) {
            return;
        }
        QuestState nextQuest = _queuedQuestStates.Dequeue();
        _currentQuestInfoDisplay.Initialize();
    }

    private void OnAllQuestsCompleted() {

    }

    public void Dispose() {
        QuestManager.Instance.OnCurrentQuestUpdated -= OnCurrentQuestUpdated;
        QuestManager.Instance.OnAllQuestsCompleted -= OnAllQuestsCompleted;
        _currentQuestInfoDisplay.OnDisplayQuestCompleteFinished -= OnQuestDisplayCompleteFinished;
        _currentQuestInfoDisplay.OnDisplayQuestFailedFinished -= OnQuestDisplayFailedFinished;


        UIManager.Instance.RemoveUIObject(QuestInfoDisplayId);
    }
}
