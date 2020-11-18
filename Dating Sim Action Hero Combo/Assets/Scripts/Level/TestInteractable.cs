using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TestInteractable : MonoBehaviour, IInteractable, PooledObject
{
    [SerializeField] private bool _interactable;
    public bool Interactable => _interactable;

    public event Action OnCompleteInteraction;

    private void Start() {
        LevelDataManager.Instance.AddOccupant(LevelDataManager.Instance.WorldToArraySpace(transform.position), this);
        LevelDataManager.Instance.RegisterInteractable(name, this);
    }

    public void InteractStart() {
        CustomLogger.Log(nameof(TestInteractable), "Started interacting with this object...");
    }

    public void InteractHold() {
        CustomLogger.Log(nameof(TestInteractable), "Continued interacting with this object...");
    }

    public void InteractEnd() {
        CustomLogger.Log(nameof(TestInteractable), "Finished interacting with this object...");
        OnCompleteInteraction?.Invoke();
    }

    public void Initialize(PooledObjectInitializationData initializationData) {

    }

    public void Dispose() {

    }

    public void Spawn() {

    }

    public void Despawn() {

    }
}
