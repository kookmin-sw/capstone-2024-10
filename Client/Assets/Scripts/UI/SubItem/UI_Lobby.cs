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
    #region Enums

    public enum Buttons
    {
        Btn_QuickStart,
        Btn_CreateGame,
        Btn_RefreshSession,
    }

    public enum GameObjects
    {
        RoomContent,
        RoomList,
    }

    public enum SubUIs
    {
        UI_RoomJoin,
        UI_RoomCreate,
    }

    #endregion

    private ILobbyController _controller;
    public GameObject RoomList { get; private set; }
    public UI_RoomCreate RoomCreate { get; private set; }
    public UI_RoomJoin RoomJoin { get; private set; }

    public override bool Init()
    {
        if (_init == true)
            return true;

        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        Bind<UI_Base>(typeof(SubUIs));

        GetButton((int)Buttons.Btn_QuickStart).onClick.AddListener(EnterGame);
        GetButton((int)Buttons.Btn_CreateGame).onClick.AddListener(CreateGame);
        GetButton((int)Buttons.Btn_RefreshSession).onClick.AddListener(Refresh);

        RoomCreate = Get<UI_Base>(SubUIs.UI_RoomCreate) as UI_RoomCreate;
        RoomCreate.Init();
        RoomCreate.gameObject.SetActive(false);

        RoomJoin = Get<UI_Base>(SubUIs.UI_RoomJoin) as UI_RoomJoin;
        RoomJoin.Init();
        RoomJoin.gameObject.SetActive(false);

        RoomList = GetObject((int)GameObjects.RoomList);

        GetButton((int)Buttons.Btn_CreateGame).interactable = false;
        GetButton((int)Buttons.Btn_QuickStart).interactable = false;
        Managers.NetworkMng.OnSessionUpdated += () => GetButton((int)Buttons.Btn_CreateGame).interactable = true;
        Managers.NetworkMng.OnSessionUpdated += () => GetButton((int)Buttons.Btn_QuickStart).interactable = true;
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
                StartCoroutine(entry.SetInfo(_controller, args));
            }
        }
    }

    public void SetInfo(ILobbyController controller)
    {
        _controller = controller;
        RoomCreate.SetInfo(controller);
        RoomJoin.SetInfo(controller, null, null);

        foreach (int i in Enum.GetValues(typeof(Buttons)))
        {
            BindEvent(GetButton(i).gameObject, (e) => {
                if (GetButton(i).interactable)
                    _controller?.PlayHover();
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
        RoomCreate.SetInfo(_controller);
        _controller.OpenRoomCreate();
    }

    void EnterGame()
    {
        _controller.ExitMenu();
        _controller.ShowLoadingMenu();
        Managers.NetworkMng.ConnectToAnySession();
    }
}
