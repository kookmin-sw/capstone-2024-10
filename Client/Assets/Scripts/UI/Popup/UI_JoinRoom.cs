using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_JoinRoom : UI_Popup
{
    #region Enums
    enum Buttons
    {
        Btn_Yes,
        Btn_No,
    }

    enum InputFields
    {
        Password,
    }

    enum Texts
    {
        Warning,
    }
    #endregion

    #region Fields
    public TMP_InputField InputPassword { get; private set; }
    private string _roomName;
    private string _password;
    private TMP_Text _warning;

    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));
        Bind<TMP_Text>(typeof(Texts));

        InputPassword = Get<TMP_InputField>((int)InputFields.Password);
        _warning = Get<TMP_Text>((int)Texts.Warning);

        Get<Button>((int)Buttons.Btn_Yes).onClick.AddListener(() =>
        {
            JoinGame();
        });

        Get<Button>((int)Buttons.Btn_No).onClick.AddListener(() =>
        {
            ClosePopupUI();
            var popup = Managers.UIMng.ShowPopupUI<UI_SessionList>(parent: transform.parent);
            popup.Init();
            popup.RefreshSessionLIst();
        });

        return true;
    }

    public void SetInfo(string roomName, string password)
    {
        _roomName = roomName;
        _password = password;
    }

    public async void JoinGame()
    {
        if (!string.IsNullOrEmpty(_password) && _password != InputPassword.text)
        {
            _warning.text = "<color=yellow>Password is incorrect</color>";
            return;
        }

        bool result = await Managers.NetworkMng.ConnectToSession(_roomName, null);
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
