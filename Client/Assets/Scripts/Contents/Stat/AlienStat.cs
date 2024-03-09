using UnityEngine;
using Fusion;

public class AlienStat: CreatureStat
{
    [Networked] public int Damage { get; set; }

    public override void SetStat(Data.CreatureData creatureData)
    {
        base.SetStat(creatureData);

        Data.AlienData alienData = (Data.AlienData)creatureData;

        Damage = alienData.Damage;
    }
}
