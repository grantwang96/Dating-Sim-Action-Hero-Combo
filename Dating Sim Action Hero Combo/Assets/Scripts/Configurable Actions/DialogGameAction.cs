using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Configured Game Actions/Dialog")]
public class DialogGameAction : ConfigurableGameAction
{
    [SerializeField] private List<DialogData> _dialogs = new List<DialogData>();

    public IReadOnlyList<DialogData> Dialogs => _dialogs;

    public override IConfiguredGameActionState CreateActionState() {
        return new DialogGameActionState(this);
    }
}

[System.Serializable]
public class DialogData {
    [SerializeField] private string _text;

    public string Text => _text;
}

public class DialogGameActionState : IConfiguredGameActionState {

    public event Action OnComplete;

    private DialogGameAction _data;

    public DialogGameActionState(DialogGameAction data) {
        _data = data;
    }

    public void Execute() {
        DialogManager.Instance.EnqueueDialog(_data.Dialogs);
        DialogManager.Instance.OnShowDialogFinished += OnAllDialogSeen;
    }

    private void OnAllDialogSeen() {
        OnComplete?.Invoke();
    }
}
