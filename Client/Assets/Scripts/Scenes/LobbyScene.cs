using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    protected override void init()
    {
        base.init();
        SceneType = Define.Scene.Lobby;

        // Managers.UI.ShowSceneUI<UI_MainScene>();
    }

    public override void Clear()
    {
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
