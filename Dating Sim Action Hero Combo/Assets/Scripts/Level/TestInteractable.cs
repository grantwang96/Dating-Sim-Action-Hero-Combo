using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestInteractable : MonoBehaviour, IInteractable, PooledObject
{
    [SerializeField] private string _interactableId;
    [SerializeField] private bool _interactable;
    public bool Interactable => _interactable;

    public event Action<IInteractable> OnCompleteInteraction;

    private void Start() {
        SetInteractableId();
        LevelDataManager.Instance.AddOccupant(LevelDataManager.Instance.WorldToArraySpace(transform.position), this);
        LevelDataManager.Instance.RegisterInteractable(name, this);
    }

    public void SetInteractable(bool interactable) {
        _interactable = interactable;
    }

    public void InteractStart() {
        CustomLogger.Log(nameof(TestInteractable), "Started interacting with this object...");
    }

    public void InteractHold() {
        CustomLogger.Log(nameof(TestInteractable), "Continued interacting with this object...");
    }

    public void InteractEnd() {
        CustomLogger.Log(nameof(TestInteractable), "Finished interacting with this object...");
        OnCompleteInteraction?.Invoke(this);
    }

    public void Initialize(PooledObjectInitializationData initializationData) {
        PooledInteractableInitData initData = initializationData as PooledInteractableInitData;
        if(initData == null) {
            return;
        }
        _interactableId = initData.OverrideId;
    }

    public void Dispose() {
        _interactableId = "";
    }

    public void Spawn() {
        gameObject.SetActive(true);
        SetInteractable(true);
    }

    public void Despawn() {
        gameObject.SetActive(false);
        SetInteractable(false);
    }

    private void SetInteractableId(string overrideId = "") {
        if (!string.IsNullOrEmpty(_interactableId)) {
            return;
        }
        if (!string.IsNullOrEmpty(overrideId)) {
            _interactableId = overrideId;
        }
        _interactableId = name;
    }
}
