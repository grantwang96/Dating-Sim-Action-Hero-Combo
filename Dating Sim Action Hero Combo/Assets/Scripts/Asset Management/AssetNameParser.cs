using System.IO;
using UnityEngine;

public class AssetNameParser {

    private const char AssetNameSplitter = '_';
    private const char AssetBundlePathSplitter = '.';

    public static bool TryParseAssetBundlePath(string assetName, out string filePath) {
        filePath = "";
        string[] assetNameComponents = assetName.Split(AssetNameSplitter);
        // this should be split into 2 parts
        if (assetNameComponents.Length != 2) {
            CustomLogger.Error(nameof(IAssetManager), $"{assetName} is an invalid asset name!");
            return false;
        }
        string[] filePathComponents = assetNameComponents[0].Split(AssetBundlePathSplitter);
        // file path components should be greater than 0
        if (filePathComponents.Length == 0) {
            return false;
        }
        filePath = filePathComponents[0];
        for (int i = 1; i < filePathComponents.Length; i++) {
            string component = filePathComponents[i].ToLower();
            filePath = $"{filePath}/{component}";
        }
        filePath = $"Assets/BundledResources/{filePath}";
        return true;
    }
}
