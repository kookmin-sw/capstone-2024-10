using System.Collections;
using UnityEngine.UI;
using TMPro;

public class UI_AlienIngame : UI_Ingame
{
    private Alien Alien {
        get => Creature as Alien;
        set => Creature = value;
    }
    enum AlienButtons
    {

    }

    enum AlienImages
    {

    }

    enum AlienTexts
    {

    }

    enum AlienSubItemUIs
    {

    }


    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<Button>(typeof(AlienButtons));
        Bind<Image>(typeof(AlienImages));
        Bind<TMP_Text>(typeof(AlienTexts));
        Bind<UI_Base>(typeof(AlienSubItemUIs));

        return true;
    }

    public override void AssignCreature(Creature creature)
    {
        base.AssignCreature(creature);
    }
}
