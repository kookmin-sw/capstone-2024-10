using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Fusion;

public class UI_Lobby : UI_Scene
{
    #region UI 목록들

    public enum Buttons
    {
        Create,
        Refresh,
        Setting,
    }

    public enum Images
    {
    }

    public enum Texts
    {
        ReceivedMessage,
        FirstBtnText,
        SecondBtnText,
    }

    public enum GameObjects
    {
        RoomContent,
        SendingMessage,
    }

    #endregion

    private TMP_InputField _input;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.Create).onClick.AddListener(CreateGame);
        GetButton((int)Buttons.Refresh).onClick.AddListener(RefreshSessionLIst);
        GetButton((int)Buttons.Setting).onClick.AddListener(GameSetting);

        _input = GetObject((int)GameObjects.SendingMessage).GetComponent<TMP_InputField>();
        GetButton((int)Buttons.Create).interactable = false;
        Managers.NetworkMng.OnSessionUpdated += () => GetButton((int)Buttons.Create).interactable = true;
        RefreshSessionLIst();

        return true;
    }

    public void RefreshSessionLIst()
    {
        foreach (Transform child in GetObject((int)GameObjects.RoomContent).transform)
        {
            Destroy(child.gameObject);
        }

        foreach (SessionInfo session in Managers.NetworkMng.Sessions)
        {
            if (session.IsVisible)
            {
                UI_SessionEntry entry = Managers.UIMng.MakeSubItem<UI_SessionEntry>(GetObject((int)GameObjects.RoomContent).transform);
                var args = new SessionEntryArgs()
                {
                    session = session
                };
                StartCoroutine(entry.SetInfo(this, args));
            }
        }
    }

    void Refresh()
    {
        StartCoroutine(RefreshWait());
    }

    IEnumerator RefreshWait()
    {
        GetButton((int)Buttons.Refresh).interactable = false;
        RefreshSessionLIst();
        yield return new WaitForSeconds(3f);
        GetButton((int)Buttons.Refresh).interactable = true;
    }

    void CreateGame()
    {
        Managers.NetworkMng.CreateSession();
        Destroy(gameObject);
    }

    void EnterGame()
    {
        Managers.SceneMng.LoadScene(Define.SceneType.GameScene);
    }

    void GameSetting()
    {

    }
}
