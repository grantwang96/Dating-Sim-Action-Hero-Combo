using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUIObject : UIObject
{
    public override void Display() {
        base.Display();
        CustomLogger.Log(nameof(TestUIObject), $"Display this!");
    }
}
