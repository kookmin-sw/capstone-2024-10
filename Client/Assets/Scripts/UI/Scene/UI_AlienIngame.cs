using System;
using UnityEngine;

public class UI_AlienIngame : UI_Ingame
{
    public UI_AlienSkill UI_AlienSkill { get; private set; }

    private Alien Alien {
        get => Creature as Alien;
        set => Creature = value;
    }

    enum AlienSubItemUIs
    {
        UI_AlienSkill,//
    }

    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<UI_Base>(typeof(AlienSubItemUIs));

        UI_AlienSkill = Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)AlienSubItemUIs.UI_AlienSkill) as UI_AlienSkill;//

        Canvas = gameObject.GetComponent<Canvas>();

        return true;
    }

    public override void InitAfterNetworkSpawn(Creature creature)
    {
        base.InitAfterNetworkSpawn(creature);

        (Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)AlienSubItemUIs.UI_AlienSkill) as UI_AlienSkill).Alien = Alien;
    }

    public override void HideUi()
    {
        base.HideUi();

        UI_AlienSkill.gameObject.SetActive(false);
    }
}
