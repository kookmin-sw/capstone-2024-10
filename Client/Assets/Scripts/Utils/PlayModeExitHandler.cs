using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public class PlayModeExitHandler
{
    static PlayModeExitHandler()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            var damageMaterial = Resources.Load<Material>("Materials/DamageMaterial");
            damageMaterial.SetFloat("_Vignette_radius", 1);
        }
    }
}
