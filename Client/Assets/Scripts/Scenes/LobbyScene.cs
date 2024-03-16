using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.LobbyScene;

        Managers.UIMng.ShowSceneUI<UI_Lobby>();

        Managers.ResourceMng.Instantiate("Cameras/LobbyCamera");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

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
