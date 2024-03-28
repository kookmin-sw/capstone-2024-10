using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;   

public class UI_Ingame : UI_Scene
{
    public Creature Creature { get; set; }
    public UI_WorkProgressBar WorkProgressBar;
    public UI_InteractInfo InteractInfo;
    protected enum Buttons
    {

    }

    protected enum Images
    {

    }

    protected enum Texts
    {

    }

    protected enum SubItemUIs
    {
        UI_WorkProgressBar,
        UI_InteractInfo,
    }


    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<UI_Base>(typeof(SubItemUIs));

        WorkProgressBar = Get<UI_Base>(SubItemUIs.UI_WorkProgressBar) as UI_WorkProgressBar;
        InteractInfo = Get<UI_Base>(SubItemUIs.UI_InteractInfo) as UI_InteractInfo;

        return true;
    }

    public virtual void AssignCreature(Creature creature)
    {
        Creature = creature;
    }

}
