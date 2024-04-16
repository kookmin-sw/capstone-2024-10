using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class UI_RoomCreate : UI_Base
{
    #region Enums
    enum Buttons
    {
        Btn_Yes,
        Btn_No,
    }

    enum InputFields
    {
        RoomName,
        Password,
    }
    #endregion

    #region Fields
    private ILobbyController _controller;
    public TMP_InputField RoomName { get; private set; }
    public TMP_InputField Password { get; private set; }
    public TMP_Text RoomNamePlaceholder { get; private set; }

    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));
        RoomName = Get<TMP_InputField>((int)InputFields.RoomName);
        Password = Get<TMP_InputField>((int)InputFields.Password);
        RoomNamePlaceholder = Util.FindChild<TMP_Text>(RoomName.gameObject, "Placeholder", true);
        Get<Button>((int)Buttons.Btn_Yes).onClick.AddListener(() =>
        {
            _controller.CloseRoomCreate();
            CreateGame();
        });
        Get<Button>((int)Buttons.Btn_No).onClick.AddListener(() =>
        {
            _controller.CloseRoomCreate();
        });

        return true;
    }

    public void SetInfo(ILobbyController controller)
    {
        _controller = controller;
        int randomInt = Random.Range(1000, 9999);
        RoomNamePlaceholder.text = "Room-" + randomInt.ToString();
        Password.text = "";
    }

    public void CreateGame()
    {
        _controller.ExitMenu();
        _controller.ShowLoadingMenu();
        string name = RoomName.text.IsNullOrEmpty() ? RoomNamePlaceholder.text : RoomName.text;
        Managers.NetworkMng.CreateSession(name, Password.text);
    }
}
