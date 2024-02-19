using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Entry : UI_Popup
{
    #region UI 목록들
    public enum Buttons
    {
        Submit,
    }

    public enum Images
    {
    }

    public enum Texts
    {
    }

    public enum GameObjects
    {
        NickName,
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.Submit).onClick.AddListener(SubmitName);

        return true;
    }

    public void SubmitName()
    {
        string name = GetObject((int)GameObjects.NickName).GetComponent<TMP_InputField>().text;
        Managers.NetworkMng.ConnectToLobby(name);
        Managers.UIMng.ClosePopupUI(this);
    }
}
