using Data;

public class AlienStat: BaseStat
{
    public Alien Alien => Creature as Alien;
    AlienData AlienData => CreatureData as AlienData;

    public int AttackDamage { get; set; }
    public int RoarSpiritDamage { get; set; }

    public override void SetStat(CreatureData creatureData)
    {
        base.SetStat(creatureData);

        AttackDamage = AlienData.AttackDamage;
        RoarSpiritDamage = AlienData.RoarSpiritDamage;
    }
}
