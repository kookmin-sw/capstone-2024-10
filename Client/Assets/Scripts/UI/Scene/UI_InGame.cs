using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Ingame : UI_Scene
{
    enum Buttons
    {

    }

    enum Images
    {
    }

    enum Texts
    {
        
    }

    enum SubItemUIs
    {
       UI_WorkProgressBar
    }


    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<UI_Base>(typeof(SubItemUIs));

        return true;
    }

    public UI_WorkProgressBar ShowWorkProgressBar(string workDescription, float requiredWorkAmount)
    {
        UI_WorkProgressBar bar = Get<UI_Base>((int)SubItemUIs.UI_WorkProgressBar) as UI_WorkProgressBar;
        bar.gameObject.SetActive(true);
        bar.SetInfo(workDescription, requiredWorkAmount);
        return bar;
    }
}
