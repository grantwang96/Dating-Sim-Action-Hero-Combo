using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurableActionsManager : MonoBehaviour
{
    public static ConfigurableActionsManager Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }
}
