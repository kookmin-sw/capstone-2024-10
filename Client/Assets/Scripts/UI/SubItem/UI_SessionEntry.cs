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
    #endregion

    private SessionInfo _session;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));

        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        GetButton(Buttons.JoinButton).onClick.AddListener(JoinSession);

        return true;
    }

    public IEnumerator SetInfo(SessionEntryArgs args)
    {
        yield return null;
        _session = args.session;

        string roomName = _session.Name;
        if (_session.Properties.TryGetValue("password", out SessionProperty password) && password != "")
        {
            roomName = "<sprite=0>   " + roomName;
        }
        else
        {
            roomName = "       " + roomName;
        }

        GetText(Texts.RoomName).text = roomName;
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

    private async void JoinSession()
    {
        if (_session.Properties.TryGetValue("password", out SessionProperty password))
        {
            Managers.UIMng.ClosePopupUIUntil<UI_Lobby>();
            var popup = Managers.UIMng.ShowPopupUI<UI_JoinRoom>(parent: Managers.UIMng.PeekPopupUI<UI_Lobby>().transform);
            popup.SetInfo(_session.Name, password);
        }
        else
        {
            Managers.UIMng.ShowPopupUI<UI_RaycastBlock>();
            bool result = await Managers.NetworkMng.ConnectToSession(_session.Name, null);
            Managers.UIMng.ClosePopupUI();

            if (result)
            {
                Managers.Clear();
                Managers.UIMng.ShowPanelUI<UI_Loading>();
            }
            else
            {
                Managers.UIMng.ClosePopupUIUntil<UI_Lobby>();
                var lobby = Managers.UIMng.PeekPopupUI<UI_Lobby>();
                Managers.UIMng.ShowPopupUI<UI_Warning>(parent : lobby.transform);
            }
        }
    }
}
