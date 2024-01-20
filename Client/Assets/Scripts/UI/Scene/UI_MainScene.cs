using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainScene : UI_Scene
{
    #region UI ¸ñ·Ïµé
    public enum Buttons
    {
    }

    public enum Images
    {
    }

    public enum Texts
    {
    }

    public enum GameObjects
    {
    }
    #endregion


    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
