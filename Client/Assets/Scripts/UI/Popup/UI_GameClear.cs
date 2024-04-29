using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_GameClear : UI_Popup
{
    enum Buttons
    {
        Quit,
    }

    enum Texts
    {
        Text1,
        Text2
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));

        GetButton((int)Buttons.Quit).onClick.AddListener(ExitGame);

        return true;
    }

    public void ExitGame()
    {
        Managers.StartMng.Player.ExitGame();
    }
}
