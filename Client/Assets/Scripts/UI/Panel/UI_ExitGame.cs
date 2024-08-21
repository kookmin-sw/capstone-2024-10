using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ExitGame : UI_CameraPanel
{
    #region Enums
    public enum Buttons
    {
        Btn_Return,
        Btn_Manual,
        Btn_Setting,
        Btn_Exit,
    }

    public enum GameObjects
    {
        EXIT,
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        var camera = transform.Find("UICamera").GetComponent<Camera>();
        var canvas = GetComponent<Canvas>();
        canvas.planeDistance = 10f;
        if (Managers.UIMng.SceneUI != null)
            Managers.UIMng.SceneUI.gameObject.SetActive(false);
        Managers.UIMng.ActivatePopupUI(false);

        GetButton(Buttons.Btn_Return).onClick.AddListener(() =>
        {
            ClosePanelUI();
        });

        GetButton(Buttons.Btn_Manual).onClick.AddListener(() =>
        {
            Managers.UIMng.ShowPanelUI<UI_GuidePanel>();
            Destroy(gameObject);
        });

        GetButton(Buttons.Btn_Setting).onClick.AddListener(() =>
        {
            Managers.UIMng.ShowPanelUI<UI_SettingPanel>();
            Destroy(gameObject);
        });

        GetObject(GameObjects.EXIT).GetOrAddComponent<UI_Exit>();
        GetButton(Buttons.Btn_Exit).onClick.AddListener(() =>
        {
            GetObject(GameObjects.EXIT).gameObject.SetActive(true);
        });

        GetObject(GameObjects.EXIT).gameObject.SetActive(false);

        return true;
    }

    class UI_Exit : UI_Base
    {
        enum Buttons
        {
            Btn_Yes,
            Btn_No1,
        }

        public override bool Init()
        {
            if (!base.Init())
                return false;

            Bind<Button>(typeof(Buttons));

            GetButton(Buttons.Btn_Yes).onClick.AddListener(() =>
            {
                Managers.StartMng.ExitGame();
            });

            GetButton(Buttons.Btn_No1).onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });

            return true;
        }
    }
}
