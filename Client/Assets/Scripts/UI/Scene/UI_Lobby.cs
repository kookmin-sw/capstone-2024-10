using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UI_Lobby : UI_Scene
{
    #region UI ¸ñ·Ïµé
    public enum Buttons
    {
        Create,
        Enter,
        Setting,
    }

    public enum Images
    {
    }

    public enum Texts
    {
        Message,
        FirstBtnText,
        SecondBtnText,
    }

    public enum GameObjects
    {
    }
    #endregion

    TMP_Text _message;
    bool IsRoomCreated 
    {
        get
        {
            return GetText((int)Texts.FirstBtnText).text == "Destroy";
        }
        set
        {
            if (value == true)
            {
                GetText((int)Texts.FirstBtnText).text = "Destroy";
                // GetButton((int)Buttons.Enter).GetComponent<Image>().color = new Color(200, 200, 200, 255);
                GetButton((int)Buttons.Enter).interactable = false;
            }
            else
            {
                GetText((int)Texts.FirstBtnText).text = "Create";
                // GetButton((int)Buttons.Enter).GetComponent<Image>().color = new Color(255, 255, 255, 255);
                GetButton((int)Buttons.Enter).interactable = true;
            }
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.Create).onClick.AddListener(() => { if (IsRoomCreated) DestroyGame(); else CreateGame(); });
        GetButton((int)Buttons.Enter).onClick.AddListener(EnterGame);
        GetButton((int)Buttons.Setting).onClick.AddListener(GameSetting);

        _message = GetText((int)Texts.Message);

        return true;
    }

    void CreateGame()
    {
        IsRoomCreated = true;
    }

    void DestroyGame()
    {
        IsRoomCreated = false;
    }

    void EnterGame()
    {
        Managers.Scene.LoadScene(Define.Scene.Game);
    }

    void GameSetting()
    {

    }
}
