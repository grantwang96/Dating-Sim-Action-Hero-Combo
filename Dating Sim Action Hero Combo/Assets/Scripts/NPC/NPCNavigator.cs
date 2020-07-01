using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCNavigator : MonoBehaviour
{
    public Vector2 MoveInput { get; set; }
    public Vector2 LookInput { get; set; }
    public IntVector3 TargetPosition { get; set; }
}
