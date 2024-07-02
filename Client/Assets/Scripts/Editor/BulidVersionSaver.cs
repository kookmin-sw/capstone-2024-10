using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;

public class BulidVersionSaver : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        SaveBundleVersion();
    }

    public static void SaveBundleVersion()
    {
        string bundleVersion = PlayerSettings.bundleVersion;
        string path = Application.streamingAssetsPath + "/bundle_version.txt";

        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        File.WriteAllText(path, bundleVersion);
        Debug.Log("Bundle version saved to " + path);
    }
}
