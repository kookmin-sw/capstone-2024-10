using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Fusion;
using SlimUI.ModernMenu;
using System;

public class UI_Lobby : UI_Popup
{
    #region Enums

    public enum Buttons
    {
        Btn_QuickStart,
        Btn_CreateGame,
        Btn_RefreshSession,
        Btn_GameTutorial,
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.Btn_QuickStart).onClick.AddListener(EnterGame);
        GetButton((int)Buttons.Btn_CreateGame).onClick.AddListener(CreateGame);
        GetButton((int)Buttons.Btn_RefreshSession).onClick.AddListener(Refresh);
        GetButton((int)Buttons.Btn_GameTutorial).onClick.AddListener(StartTutorial);

        GetButton((int)Buttons.Btn_CreateGame).interactable = false;
        GetButton((int)Buttons.Btn_QuickStart).interactable = false;
        GetButton((int)Buttons.Btn_GameTutorial).interactable = false;
        Managers.NetworkMng.OnSessionUpdated += () => GetButton((int)Buttons.Btn_CreateGame).interactable = true;
        Managers.NetworkMng.OnSessionUpdated += () => GetButton((int)Buttons.Btn_QuickStart).interactable = true;
        Managers.NetworkMng.OnSessionUpdated += () => GetButton((int)Buttons.Btn_GameTutorial).interactable = true;

        var popup = Managers.UIMng.ShowPopupUI<UI_SessionList>(parent: transform);
        popup.Init();
        popup.RefreshSessionLIst();

        return true;
    }

    
    public void SetInfo(UI_LobbyController controller)
    {
        foreach (int i in Enum.GetValues(typeof(Buttons)))
        {
            BindEvent(GetButton(i).gameObject, (e) => {
                if (GetButton(i).interactable)
                    controller?.PlayHover();
            }, Define.UIEvent.PointerEnter);
        }
    }

    void Refresh()
    {
        StartCoroutine(RefreshWait());
    }

    IEnumerator RefreshWait()
    {
        Managers.UIMng.ClosePopupUIUntil<UI_Lobby>();
        var popup = Managers.UIMng.ShowPopupUI<UI_SessionList>(parent : transform);
        popup.Init();

        GetButton((int)Buttons.Btn_RefreshSession).interactable = false;
        popup.RefreshSessionLIst();
        yield return new WaitForSeconds(1f);
        GetButton((int)Buttons.Btn_RefreshSession).interactable = true;
    }

    void CreateGame()
    {
        Managers.UIMng.ClosePopupUIUntil<UI_Lobby>();
        var popup = Managers.UIMng.ShowPopupUI<UI_CreateRoom>(parent: transform);
        popup.Init();
        popup.SetInfo();
    }

    void EnterGame()
    {
        Managers.UIMng.Clear();
        Managers.NetworkMng.ConnectToAnySession();
        Managers.UIMng.ShowPanelUI<UI_Loading>();
    }

    void StartTutorial()
    {
        Managers.Clear();
        Managers.NetworkMng.StartSharedClient(Define.SceneType.TutorialScene);
        Managers.UIMng.ShowPanelUI<UI_Loading>();
    }
}
