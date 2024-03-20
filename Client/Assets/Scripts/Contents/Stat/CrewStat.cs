using UnityEngine;
using Fusion;

public class CrewStat : BaseStat
{
    [Networked] public int Hp { get; set; }
    [Networked] public int MaxHp { get; set; }
    [Networked] public float Stamina { get; set; }
    [Networked] public float MaxStamina { get; set; }
    [Networked] public float SitSpeed { get; set; }
    [Networked] public float WorkSpeed { get; set; }

    public override void SetStat(Data.CreatureData creatureData)
    {
        base.SetStat(creatureData);

        Data.CrewData crewData = (Data.CrewData)creatureData;

        Hp = crewData.MaxHp;
        MaxHp = crewData.MaxHp;
        Stamina = crewData.Stamina;
        MaxStamina = crewData.Stamina;
        SitSpeed = crewData.SitSpeed;
        WorkSpeed = crewData.WorkSpeed;
    }

    #region Event

    public void OnDamage(int value)
    {
        if (value < 0)
        {
            Debug.Log("Invalid OnDamage");
            return;
        }

        Hp = Mathf.Clamp(Hp - value, 0, MaxHp);
    }

    public void OnHeal(int value)
    {
        if (value < 0)
        {
            Debug.Log("Invalid OnHeal");
            return;
        }

        Hp = Mathf.Clamp(Hp + value, 0, MaxHp);
    }

    public void OnUseStamina(float value)
    {
        if (value < 0)
        {
            Debug.Log("Invalid OnUseStamina");
            return;
        }

        Stamina = Mathf.Clamp(Stamina - value, 0, MaxStamina);
    }

    public void OnRecoverStamina(float value)
    {
        if (value < 0)
        {
            Debug.Log("Invalid OnRecoverStamina");
            return;
        }

        Stamina = Mathf.Clamp(Stamina + value, 0, MaxStamina);
    }

    #endregion
}
