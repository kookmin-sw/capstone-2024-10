using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_InGame : UI_Scene
{
    public enum Buttons
    {

    }

    public enum Images
    {
    }

    public enum Texts
    {
        
    }

    public enum GameObjects
    {
        //Electric_Control,
        //ShowLongWork,
    }


    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        /*
        GetObject((int)GameObjects.Electric_Control).SetActive(false);
        GetObject((int)GameObjects.ShowLongWork).SetActive(false);
        */
        return true;
    }
}
