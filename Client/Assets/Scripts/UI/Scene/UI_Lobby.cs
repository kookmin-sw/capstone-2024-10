using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Fusion;
using SlimUI.ModernMenu;
using System;

public class UI_Lobby : UI_Base
{
    #region UI 목록들

    public enum Buttons
    {
        Btn_QuickStart,
        Btn_CreateGame,
        Btn_RefreshSession,
    }

    public enum GameObjects
    {
        RoomContent,
    }

    #endregion

    public UI_LobbyController UIMenuController { get; private set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.Btn_QuickStart).onClick.AddListener(EnterGame);
        GetButton((int)Buttons.Btn_CreateGame).onClick.AddListener(CreateGame);
        GetButton((int)Buttons.Btn_RefreshSession).onClick.AddListener(Refresh);

        GetButton((int)Buttons.Btn_CreateGame).interactable = false;
        Managers.NetworkMng.OnSessionUpdated += () => GetButton((int)Buttons.Btn_CreateGame).interactable = true;
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

    public void SetInfo(UI_LobbyController controller)
    {
        UIMenuController = controller;
        foreach (int i in Enum.GetValues(typeof(Buttons)))
        {
            BindEvent(GetButton(i).gameObject, (e) => {
                if (GetButton(i).interactable)
                    UIMenuController?.PlayHover();
            }, Define.UIEvent.PointerEnter);
        }
    }

    void Refresh()
    {
        StartCoroutine(RefreshWait());
    }

    IEnumerator RefreshWait()
    {
        GetButton((int)Buttons.Btn_RefreshSession).interactable = false;
        RefreshSessionLIst();
        yield return new WaitForSeconds(3f);
        GetButton((int)Buttons.Btn_RefreshSession).interactable = true;
    }

    void CreateGame()
    {
        Managers.NetworkMng.CreateSession();
        Destroy(gameObject);
    }

    void EnterGame()
    {

    }
}
