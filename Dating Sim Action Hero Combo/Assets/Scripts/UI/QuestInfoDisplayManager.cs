using System;
using System.Collections.Generic;

public class QuestInfoDisplayManager : IInitializableManager {

    private const string QuestInfoDisplayId = "QuestInfoDisplay";

    public static QuestInfoDisplayManager Instance => GetOrSetInstance();
    private static QuestInfoDisplayManager _instance;

    private QuestInfoDisplay _currentQuestInfoDisplay; // the current objective for the 

    private static QuestInfoDisplayManager GetOrSetInstance() {
        if(_instance == null) {
            _instance = new QuestInfoDisplayManager();
        }
        return _instance;
    }

    public void Initialize(Action<bool> initializationCallback = null) {
        GameEventsManager.StartGame?.Subscribe(OnGameStart);
        GameEventsManager.ExitGame?.Subscribe(OnExitGame);
        GameEventsManager.EndGameResults?.Subscribe(OnEnterEndGameResults);
        CustomLogger.Log(nameof(QuestManager), $"Initializing {nameof(QuestInfoDisplayManager)}");
        initializationCallback?.Invoke(true);
    }

    private void OnGameStart() {
        if (_currentQuestInfoDisplay == null) {
            // generate the current quest info display
            UIObject uiObject = UIManager.Instance.CreateNewUIObject(QuestInfoDisplayId, UILayerId.Notifications);
            _currentQuestInfoDisplay = uiObject as QuestInfoDisplay;
            if (_currentQuestInfoDisplay == null) {
                CustomLogger.Error(nameof(QuestInfoDisplayManager), $"Could not create current quest info display with id: {QuestInfoDisplayId}");
                return;
            }
        }
        _currentQuestInfoDisplay.Initialize();
        _currentQuestInfoDisplay.Display();
    }

    private void OnEnterEndGameResults() {
        _currentQuestInfoDisplay?.Hide();
    }

    private void OnExitGame() {
        _currentQuestInfoDisplay?.Hide();
    }
    
    public void Dispose() {
        GameEventsManager.StartGame?.Unsubscribe(OnGameStart);
        GameEventsManager.EndGameResults?.Unsubscribe(OnEnterEndGameResults);
        GameEventsManager.ExitGame?.Unsubscribe(OnExitGame);
        UIManager.Instance.RemoveUIObject(QuestInfoDisplayId);
    }
}