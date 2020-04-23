using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action OnGameStarted;
    public event Action OnGameEnded;

    private void Awake() {
        Instance = this;
    }

    public void StartGame() {
        OnGameStarted?.Invoke();
    }

    public void EndGame() {
        OnGameEnded?.Invoke();
    }
}
