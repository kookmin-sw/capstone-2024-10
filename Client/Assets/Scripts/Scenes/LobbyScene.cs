using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        SceneType = Define.SceneType.LobbyScene;
    }

    private void Start()
    {
        Managers.UIMng.ShowSceneUI<UI_Lobby>();
        string nickname = FusionConnection.Instance.PlayerName;
        if (string.IsNullOrEmpty(nickname))
        {
            Managers.UIMng.ShowPopupUI<UI_Entry>();
        }
        else
        {
            FusionConnection.Instance.ConnectToLobby(nickname);
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
