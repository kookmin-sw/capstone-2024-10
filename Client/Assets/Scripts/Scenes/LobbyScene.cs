using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.LobbyScene;

        Managers.ResourceMng.Instantiate("Camera");
        Managers.UIMng.ShowSceneUI<UI_Lobby>();

        string nickname = Managers.NetworkMng.PlayerName;
        if (string.IsNullOrEmpty(nickname))
        {
            Managers.UIMng.ShowPopupUI<UI_Entry>();
        }
        else
        {
            Managers.NetworkMng.ConnectToLobby(nickname);
        }
    }

    public override void Clear()
    {
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
