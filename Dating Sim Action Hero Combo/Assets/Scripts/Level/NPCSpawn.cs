using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawn : MonoBehaviour
{
    private void Awake() {
        LevelDataManager.Instance.RegisterNPCSpawnPoint(name, transform);
    }
}
