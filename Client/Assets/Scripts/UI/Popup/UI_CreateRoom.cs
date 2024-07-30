using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

public class UI_CreateRoom : UI_Popup
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
    public TMP_InputField RoomName { get; private set; }
    public TMP_InputField Password { get; private set; }
    public TMP_Text RoomNamePlaceholder { get; private set; }

    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        transform.localPosition = new Vector3(47, 317, 0);

        Bind<Button>(typeof(Buttons));
        Bind<TMP_InputField>(typeof(InputFields));
        RoomName = Get<TMP_InputField>((int)InputFields.RoomName);
        Password = Get<TMP_InputField>((int)InputFields.Password);
        RoomNamePlaceholder = Util.FindChild<TMP_Text>(RoomName.gameObject, "Placeholder", true);
        Get<Button>((int)Buttons.Btn_Yes).onClick.AddListener(() =>
        {
            ClosePopupUI();
            CreateGame();
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

    public void SetInfo()
    {
        int randomInt = Random.Range(1000, 9999);
        RoomNamePlaceholder.text = "Room-" + randomInt.ToString();
        Password.text = "";
    }

    public async void CreateGame()
    {
        string roomName = RoomName.text.Trim();

        // 입력 가능 글자 14글자로 제한
        roomName = roomName.IsNullOrEmpty() ? RoomNamePlaceholder.text : roomName.Substring(0, 14);

        Managers.UIMng.ShowPopupUI<UI_RaycastBlock>();
        bool result = await Managers.NetworkMng.CreateSession(roomName, Password.text);
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
