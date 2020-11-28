using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CivilianUnit : NPCUnit, IInteractable {

    public bool Interactable { get; private set; } = true;

    [SerializeField] private List<DialogData> _onInteractDialogs = new List<DialogData>();

    public event Action<IInteractable> OnCompleteInteraction;

    public override void Initialize(PooledObjectInitializationData initializationData) {
        base.Initialize(initializationData);
        CivilianInitializationData initData = initializationData as CivilianInitializationData;
        if(initData == null) {
            return;
        }
        _onInteractDialogs.Clear();
        _onInteractDialogs.AddRange(initData.DialogDatas);
    }

    public override void Spawn() {
        gameObject.SetActive(true);
    }

    public override void Despawn() {
        gameObject.SetActive(false);
    }

    public void SetInteractable(bool interactable) {
        Interactable = interactable;
    }

    public void InteractEnd() {

    }

    public void InteractHold() {

    }

    public void InteractStart() {
        DialogManager.Instance.OnShowDialogFinished += OnShowDialogFinished;
        DialogManager.Instance.EnqueueDialog(new List<DialogData>(_onInteractDialogs));
    }

    private void OnShowDialogFinished() {
        DialogManager.Instance.OnShowDialogFinished -= OnShowDialogFinished;
        OnCompleteInteraction?.Invoke(this);
    }
    
    protected override void SubscribeToAllianceManager() {

    }

    protected override void UnsubscribeToAllianceManager() {

    }
}

public class CivilianInitializationData : UnitInitializationData{
    public DialogData[] DialogDatas;
}
