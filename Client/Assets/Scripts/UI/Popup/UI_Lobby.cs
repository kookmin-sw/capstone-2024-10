using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
        SearchIcon,
    }

    public enum Inputs
    {
        Search,
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(Inputs));

        GetButton((int)Buttons.Btn_QuickStart).onClick.AddListener(EnterGame);
        GetButton((int)Buttons.Btn_CreateGame).onClick.AddListener(CreateGame);
        GetButton((int)Buttons.Btn_RefreshSession).onClick.AddListener(Refresh);
        GetButton((int)Buttons.SearchIcon).onClick.AddListener(Refresh);
        GetButton((int)Buttons.Btn_GameTutorial).onClick.AddListener(StartTutorial);

        EnableUIInteraction(false);
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

    public void Refresh()
    {
        StartCoroutine(RefreshWait());
    }

    private IEnumerator RefreshWait()
    {
        Managers.UIMng.ClosePopupUIUntil<UI_Lobby>();
        var popup = Managers.UIMng.ShowPopupUI<UI_SessionList>(parent : transform);
        popup.Init();

        GetButton((int)Buttons.Btn_RefreshSession).interactable = false;
        popup.RefreshSessionLIst(Get<TMP_InputField>(Inputs.Search).text.Trim());
        yield return new WaitForSeconds(1f);
        GetButton((int)Buttons.Btn_RefreshSession).interactable = true;
    }

    private void EnableUIInteraction(bool toggle)
    {
        GetButton((int)Buttons.Btn_CreateGame).interactable = toggle;
        GetButton((int)Buttons.Btn_QuickStart).interactable = toggle;
        GetButton((int)Buttons.Btn_GameTutorial).interactable = toggle;
    }

    private void CreateGame()
    {
        Managers.UIMng.ClosePopupUIUntil<UI_Lobby>();
        var popup = Managers.UIMng.ShowPopupUI<UI_CreateRoom>(parent: transform);
        popup.Init();
        popup.SetInfo();
    }

    private async void EnterGame()
    {
        Managers.UIMng.ShowPopupUI<UI_RaycastBlock>();
        bool result = await Managers.NetworkMng.ConnectToAnySession();
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

    private void StartTutorial()
    {
        Managers.Clear();
        Managers.NetworkMng.StartTutorialSharedClient();
        Managers.UIMng.ShowPanelUI<UI_Loading>();
    }
}
