using System.Collections;
using System.Collections.Generic;
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

        transform.localPosition = new Vector3(47, 227, 0);

        InputPassword = Get<TMP_InputField>((int)InputFields.Password);
        _warning = Get<TMP_Text>((int)Texts.Warning);

        Get<Button>((int)Buttons.Btn_Yes).onClick.AddListener(() =>
        {
            if (JoinGame())
            {
                ClosePopupUI();
                Managers.UIMng.ShowPanelUI<UI_Loading>();
            }
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

    public bool JoinGame()
    {
        if (!string.IsNullOrEmpty(_password) && _password != InputPassword.text)
        {
            _warning.text = "Password is incorrect";
            return false;
        }

        Managers.NetworkMng.ConnectToSession(_roomName, null);
        return true;
    }
}
