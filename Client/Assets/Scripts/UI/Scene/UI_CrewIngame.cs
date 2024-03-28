using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UI_CrewIngame : UI_Ingame
{
    private Crew Crew {
        get => Creature as Crew;
        set => Creature = value;
    }
    enum CrewButtons
    {

    }

    enum CrewImages
    {

    }

    enum CrewTexts
    {

    }

    enum CrewSubItemUIs
    {
       UI_CrewHP,
       UI_CrewStamina
    }


    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<Button>(typeof(CrewButtons));
        Bind<Image>(typeof(CrewImages));
        Bind<TMP_Text>(typeof(CrewTexts));
        Bind<UI_Base>(typeof(CrewSubItemUIs));

        return true;
    }

    public override void AssignCreature(Creature creature)
    {
        base.AssignCreature(creature);
 
        (Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)CrewSubItemUIs.UI_CrewHP) as UI_CrewHP).Crew = Crew;
        (Get<UI_Base>(Enum.GetNames(typeof(SubItemUIs)).Length + (int)CrewSubItemUIs.UI_CrewStamina) as UI_CrewStamina).Crew = Crew;
    }
}
