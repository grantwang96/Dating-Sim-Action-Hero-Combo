using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PooledObject
{
    void Initialize(PooledObjectInitializationData initializationData);
    void Spawn();
    void Despawn();
}

public class PooledObjectInitializationData {

}