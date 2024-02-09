using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;

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
    }
    #endregion

    UI_Base _parent;
    SessionInfo _session;

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

        GetButton((int)Buttons.JoinButton).onClick.AddListener(JoinSession);

        return true;
    }

    public IEnumerator SetInfo(UI_Lobby parent, SessionEntryArgs args)
    {
        yield return null;
        _parent = parent;
        _session = args.session;
        GetText((int)Texts.RoomName).text = _session.Name;
        GetText((int)Texts.PlayerCount).text = _session.PlayerCount + "/" + _session.MaxPlayers;
        if (_session.IsOpen == false || _session.PlayerCount >= _session.MaxPlayers)
        {
            GetButton((int)Buttons.JoinButton).interactable = false;
        }
        else
        {
            GetButton((int)Buttons.JoinButton).interactable = true;
        }
    }

    private void JoinSession()
    {
        FusionConnection.instance.ConnectToSession(GetText((int)Texts.RoomName).text);
        _parent.gameObject.SetActive(false);
    }
}
