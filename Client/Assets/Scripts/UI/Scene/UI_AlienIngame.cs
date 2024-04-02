using System.Collections;
using UnityEngine.UI;
using TMPro;

public class UI_AlienIngame : UI_Ingame
{
    private Alien Alien {
        get => Creature as Alien;
        set => Creature = value;
    }

    enum AlienSubItemUIs
    {

    }


    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<UI_Base>(typeof(AlienSubItemUIs));

        return true;
    }

    public override void InitAfterNetworkSpawn(Creature creature)
    {
        base.InitAfterNetworkSpawn(creature);
    }
}
