using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PooledObject
{
    void Initialize(PooledObjectInitializationData initializationData); // initializes and enables object
    void Dispose(); // cleans up pooled object
    void Spawn(); // creates the object for use
    void Despawn(); // removes the object for use
}

public class PooledObjectInitializationData {

}