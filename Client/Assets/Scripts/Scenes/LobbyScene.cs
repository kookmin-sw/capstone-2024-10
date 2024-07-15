using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.LobbyScene;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Managers.SoundMng.Play($"{Define.BGM_PATH}/Black Magic", Define.SoundType.Bgm, 0.4f, 0.7f);

        int randomInt = Random.Range(1000, 9999);
        string nickname = "User-" + randomInt.ToString();
        Managers.NetworkMng.ConnectToLobby(nickname);

        SettingSystem settingSystem = FindAnyObjectByType<SettingSystem>();
        settingSystem.Init();
    }

    public override void Clear()
    {
        FindObjectOfType<UI_LobbyController>()?.DestroyMenu();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
