﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MyAssetPostProcessor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
        /*
        foreach (string str in importedAssets) {
            Debug.Log("Reimported Asset: " + str);
        }
        foreach (string str in deletedAssets) {
            Debug.Log("Deleted Asset: " + str);
        }

        for (int i = 0; i < movedAssets.Length; i++) {
            Debug.Log("Moved Asset: assfart" + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }
        */
    }
}
