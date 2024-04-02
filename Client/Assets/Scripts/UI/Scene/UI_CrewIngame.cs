using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UI_CrewIngame : UI_Ingame
{
    public UI_CrewObjective ObjectiveUI { get; private set; }
    public UI_Inventory UI_Inventory { get; private set; }

    private Crew Crew {
        get => Creature as Crew;
        set => Creature = value;
    }

    enum CrewSubItemUIs
    {
       UI_CrewHP,
       UI_CrewStamina,
       UI_Inventory,
       UI_CrewObjective,
    }


    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<UI_Base>(typeof(CrewSubItemUIs));

        ObjectiveUI = Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)CrewSubItemUIs.UI_CrewObjective) as UI_CrewObjective;
        UI_Inventory = Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)CrewSubItemUIs.UI_Inventory) as UI_Inventory;

        return true;
    }

    public override void InitAfterNetworkSpawn(Creature creature)
    {
        base.InitAfterNetworkSpawn(creature);
 
        (Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)CrewSubItemUIs.UI_CrewHP) as UI_CrewHP).Crew = Crew;
        (Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)CrewSubItemUIs.UI_CrewStamina) as UI_CrewStamina).Crew = Crew;

        ObjectiveUI.UpdateUI(0);
    }
}
