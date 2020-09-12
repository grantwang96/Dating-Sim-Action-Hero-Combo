using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for all UI objects that are displayed
/// </summary>
public abstract class UIObject : MonoBehaviour
{
    // used for setting up the ui object before it is displayed
    public virtual bool Initialize() {
        return true;
    }

    // clean up the ui object before it is remove(destroy)
    public virtual void CleanUp() {

    }

    public virtual void Display() {

    }

    public virtual void Hide() {

    }
}

public class UIObjectInitializationData {

}
