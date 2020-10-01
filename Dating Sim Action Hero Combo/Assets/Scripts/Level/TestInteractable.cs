using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _interactable;
    public bool Interactable => _interactable;

    private void Start() {
        LevelDataManager.Instance.AddOccupant(LevelDataManager.Instance.WorldToArraySpace(transform.position), this);
    }

    public void InteractStart() {
        CustomLogger.Log(nameof(TestInteractable), "Started interacting with this object...");
    }

    public void InteractHold() {
        CustomLogger.Log(nameof(TestInteractable), "Continued interacting with this object...");
    }

    public void InteractEnd() {
        CustomLogger.Log(nameof(TestInteractable), "Finished interacting with this object...");
    }
}
