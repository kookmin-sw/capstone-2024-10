using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StartGame : UI_Scene
{
    enum Buttons
    {
        ReadyGame,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));

        GetButton((int)Buttons.ReadyGame).onClick.AddListener(ReadyGame);

        return true;
    }

    public void ReadyGame()
    {
    }
}
