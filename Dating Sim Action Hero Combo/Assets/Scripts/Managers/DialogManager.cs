using System.Collections;
using System.Collections.Generic;
using System;

public class DialogManager : IInitializableManager
{
    private const string UIDialogPrefabId = "UIDialog";

    public static DialogManager Instance => GetOrSetInstance();
    private static DialogManager _instance;

    public DialogData CurrentDialogData { get; private set; }

    private readonly Queue<DialogData> _dialogsToShow = new Queue<DialogData>();
    private UIDialog _uiDialog;

    public event Action OnDialogQueued;
    public event Action OnShowDialogFinished;

    private static DialogManager GetOrSetInstance() {
        if(_instance == null) {
            _instance = new DialogManager();
        }
        return _instance;
    }

    public void Initialize(Action<bool> initializationCallback = null) {

        GameEventsManager.EndGame.Subscribe(OnGameEnded);
        GameEventsManager.PauseMenu.Subscribe(OnGamePaused);

        UIObject uiObject = UIManager.Instance.CreateNewUIObject(UIDialogPrefabId, UILayerId.Overlay);
        if(uiObject == null) {
            initializationCallback?.Invoke(false);
            return;
        }
        _uiDialog = uiObject as UIDialog;
        if(_uiDialog == null) {
            initializationCallback?.Invoke(false);
            return;
        }
        _uiDialog.Initialize();
        HideCurrentDialog();
        _uiDialog.OnContinue += OnDialogContinue;

        initializationCallback?.Invoke(true);
    }

    public void Dispose() {
        GameEventsManager.EndGame.Unsubscribe(OnGameEnded);
        GameEventsManager.PauseMenu.Unsubscribe(OnGamePaused);

        _uiDialog.OnContinue -= OnDialogContinue;
        HideCurrentDialog();
        UIManager.Instance.RemoveUIObject(UIDialogPrefabId);
    }

    public void EnqueueDialog(IReadOnlyList<DialogData> dialogsToQueue) {
        for (int i = 0; i < dialogsToQueue.Count; i++) {
            _dialogsToShow.Enqueue(dialogsToQueue[i]);
        }
        // if dialog isn't currently being displayed
        if(CurrentDialogData == null) {
            ShowNextDialog();
            OnDialogQueued?.Invoke();
        }
    }

    private void ShowNextDialog() {
        if(_dialogsToShow.Count == 0) {
            ShowDialogFinished();
            return;
        }
        CurrentDialogData = _dialogsToShow.Dequeue();
        _uiDialog.Display();
    }

    private void OnGameEnded(EndGameContext endGameContext) {
        HideCurrentDialog();
    }

    private void OnGamePaused(bool paused) {
        HideCurrentDialog();
    }

    private void HideCurrentDialog() {
        _uiDialog.Hide();
    }

    private void ShowDialogFinished() {
        HideCurrentDialog();
        CurrentDialogData = null;
        OnShowDialogFinished?.Invoke();
    }

    private void OnDialogContinue() {
        ShowNextDialog();
    }
}

