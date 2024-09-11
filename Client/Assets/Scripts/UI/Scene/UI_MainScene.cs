// Application 빌드할 때 필요
using UnityEngine;

public class UI_MainScene : UI_Scene
{
    #region UI 목록들
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
