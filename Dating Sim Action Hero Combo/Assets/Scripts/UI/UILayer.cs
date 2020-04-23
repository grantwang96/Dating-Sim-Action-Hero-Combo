using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UILayerId {
    HUD,
    Panels,
    Notifications
}

public class UILayer : MonoBehaviour
{
    [SerializeField] private UILayerId _uiLayerId;
    public UILayerId UILayerId => _uiLayerId;
}
