using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using Unity.VisualScripting;

public class SessionEntryArgs
{
    public SessionInfo session { get; set; }
}

public class UI_SessionEntry : UI_Base
{
    #region UI 목록들
    public enum Buttons
    {
        JoinButton,
    }

    public enum Images
    {
    }

    public enum Texts
    {
        RoomName,
        PlayerCount,
    }

    public enum GameObjects
    {
        LockIcon,
    }
    #endregion

    private SessionInfo _session;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        GetObject(GameObjects.LockIcon).gameObject.SetActive(false);
        GetButton(Buttons.JoinButton).onClick.AddListener(JoinSession);

        return true;
    }

    public IEnumerator SetInfo(SessionEntryArgs args)
    {
        yield return null;
        _session = args.session;
        if (_session.Properties.TryGetValue("password", out SessionProperty password) && password != "")
        {
            GetObject(GameObjects.LockIcon).gameObject.SetActive(true);
        }

        GetText(Texts.RoomName).text = _session.Name;
        GetText(Texts.PlayerCount).text = _session.PlayerCount + "/" + _session.MaxPlayers;
        if (_session.IsOpen == false || _session.PlayerCount >= _session.MaxPlayers)
        {
            GetButton(Buttons.JoinButton).interactable = false;
        }
        else
        {
            GetButton(Buttons.JoinButton).interactable = true;
        }
    }

    private void JoinSession()
    {
        if (_session.Properties.TryGetValue("password", out SessionProperty password))
        {
            Managers.UIMng.ClosePopupUIUntil<UI_Lobby>();
            var popup = Managers.UIMng.ShowPopupUI<UI_JoinRoom>(parent: Managers.UIMng.PeekPopupUI<UI_Lobby>().transform);
            popup.SetInfo(GetText(Texts.RoomName).text, password);
        }
        else
        {
            Managers.UIMng.Clear();
            Managers.UIMng.ShowPanelUI<UI_Loading>(parent: Camera.main.transform);
            Managers.NetworkMng.ConnectToSession(GetText(Texts.RoomName).text, null);
        }
    }
}
