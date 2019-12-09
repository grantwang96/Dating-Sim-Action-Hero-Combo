using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AssetBundler : MonoBehaviour
{
    public const string AssetBundlePath = "Assets/BundledResources";

    [MenuItem("Asset Bundles/Build Asset Bundles - Normal")]
    static void BuildAssetBundles() {
        BuildPipeline.BuildAssetBundles(AssetBundlePath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}