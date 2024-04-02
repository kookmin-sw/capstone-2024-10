using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;   

public class UI_Ingame : UI_Scene
{
    public Creature Creature { get; set; }
    public UI_WorkProgressBar WorkProgressBarUI { get; private set; }
    public UI_InteractInfo InteractInfoUI { get; private set; }

    protected enum SubItemUIs
    {
        UI_WorkProgressBar,
        UI_InteractInfo,
    }

    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<UI_Base>(typeof(SubItemUIs));

        WorkProgressBarUI = Get<UI_Base>(SubItemUIs.UI_WorkProgressBar) as UI_WorkProgressBar;
        InteractInfoUI = Get<UI_Base>(SubItemUIs.UI_InteractInfo) as UI_InteractInfo;

        return true;
    }

    public virtual void InitAfterNetworkSpawn(Creature creature)
    {
        Creature = creature;
    }

}
