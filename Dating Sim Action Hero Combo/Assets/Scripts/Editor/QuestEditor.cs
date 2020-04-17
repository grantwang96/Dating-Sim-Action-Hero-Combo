using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Quest), true)]
[CanEditMultipleObjects]
public class QuestEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
    }
}
