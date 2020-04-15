using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private bool _interactable;
    public bool Interactable => _interactable;

    private void Start() {
        LevelDataManager.Instance.SetOccupant(LevelDataManager.Instance.WorldToArraySpace(transform.position), this);
    }

    public void Interact() {
        CustomLogger.Log(nameof(TestInteractable), "Interacted with this object!");
    }
}
