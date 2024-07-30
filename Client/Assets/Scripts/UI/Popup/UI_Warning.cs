using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Warning : UI_Popup
{
    enum Buttons
    {
        Btn_Yes,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        GetButton(Buttons.Btn_Yes).onClick.AddListener(() =>
        {
            Managers.UIMng.ClosePopupUIUntil<UI_Lobby>();
            var ui = Managers.UIMng.PeekPopupUI<UI_Lobby>();
            ui.Refresh();
        });

        return true;
    }
}
