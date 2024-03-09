using UnityEngine;
using Fusion;

public class CrewStat : CreatureStat
{
    [Networked] public int Hp { get; set; }
    [Networked] public int MaxHp { get; set; }
    [Networked] public float Stamina { get; set; }
    [Networked] public float SitSpeed { get; set; }

    public override void SetStat(Data.CreatureData creatureData)
    {
        base.SetStat(creatureData);

        Data.CrewData crewData = (Data.CrewData)creatureData;

        Hp = crewData.MaxHp;
        MaxHp = crewData.MaxHp;
        Stamina = crewData.Stamina;
        SitSpeed = crewData.SitSpeed;
    }

    #region Event
    public void OnDamage(int damage, int attackCount = 1)
    {
        if (damage < 0)
        {
            Debug.Log("Invalid OnDamage");
            return;
        }

        Hp = Mathf.Clamp(Hp - damage, 0, MaxHp);
    }

    public void OnHeal(int heal)
    {
        if (heal < 0)
        {
            Debug.Log("Invalid OnHeal");
            return;
        }

        Hp = Mathf.Clamp(Hp + heal, 0, MaxHp);
    }
    #endregion
}
