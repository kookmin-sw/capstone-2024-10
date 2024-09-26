using UnityEngine;

public class LobbyScene : BaseScene
{
    static bool IsFirstLobby = true;
    public UI_LobbyScene SceneUI { get; private set; }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.LobbyScene;

        SettingSystem settingSystem = FindAnyObjectByType<SettingSystem>();
        settingSystem.Init();

        string nickname = Managers.GameMng.SettingSystem.GetName();
        Managers.NetworkMng.ConnectToLobby(nickname);

        UI_Introduction intro;
        if (IsFirstLobby)
        {
            intro = Managers.UIMng.ShowPanelUI<UI_Introduction>();
            intro.Init();
            intro.StartFade();
            intro.onFinished += () => Destroy(intro.gameObject);
            intro.onFinished += InitAfterIntroduction;
            IsFirstLobby = false;
        }
        else
        {
            InitAfterIntroduction();
        }
    }

    public void InitAfterIntroduction()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Managers.SoundMng.Play($"{Define.BGM_PATH}/Black Magic", Define.SoundType.Bgm, 0.4f, 0.7f);
        SceneUI = Managers.UIMng.ShowSceneUI<UI_LobbyScene>();
        SceneUI.Init();
        SceneUI.ShowServerInitializeMessage();
        Managers.NetworkMng.OnSessionUpdated += () => SceneUI.HideServerInitializeMessage();
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
