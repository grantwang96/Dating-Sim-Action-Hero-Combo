using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtraMath
{
    public static Vector2 AngleToVector2(float angleInDegrees) {
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), -Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
