using UnityEngine;
using UnityEditor;

public class PlayerPrefsEditor : EditorWindow
{
    [MenuItem("Tools/Reset PlayerPrefs")]
    public static void ResetPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog("Reset PlayerPrefs", "Are you sure you want to reset all PlayerPrefs data?", "Yes", "No"))
        {
            PlayerPrefs.DeleteAll();
            Debug.Log("PlayerPrefs data has been reset.");
        }
    }
}
