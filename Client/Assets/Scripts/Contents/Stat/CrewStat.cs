using Data;
using UnityEngine;

public class CrewStat : BaseStat
{
    public Crew Crew => Creature as Crew;
    CrewData CrewData => CreatureData as CrewData;

    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public float Stamina { get; set; }
    public float MaxStamina { get; set; }
    public float Sanity { get; set; }
    public float MaxSanity { get; set; }
    public float SitSpeed { get; set; }

    public bool IsRunnable { get; set; }

    public override void SetStat(CreatureData creatureData)
    {
        base.SetStat(creatureData);

        Hp = CrewData.MaxHp;
        MaxHp = CrewData.MaxHp;
        Stamina = CrewData.MaxStamina;
        MaxStamina = CrewData.MaxStamina;
        Sanity = CrewData.MaxSanity;
        MaxSanity = CrewData.MaxSanity;
        SitSpeed = CrewData.SitSpeed;
        IsRunnable = true;
    }

    #region Event

    public void OnHpChanged(int value)
    {
        Hp = Mathf.Clamp(Hp + value, 0, MaxHp);
    }

    public void OnStaminaChanged(float value)
    {
        Stamina = Mathf.Clamp(Stamina + value, 0, MaxStamina);

        if (Stamina >= 20)
            IsRunnable = true;
        else if (Stamina <= 0)
            IsRunnable = false;
    }

    public void OnSanityChanged(float value)
    {
        Sanity = Mathf.Clamp(Sanity + value, 0, MaxSanity);

        if (Sanity >= 70)
        {
            WalkSpeed = CrewData.WalkSpeed;
            RunSpeed = CrewData.RunSpeed;
            SitSpeed = CrewData.SitSpeed;
        }
        else if (Sanity < 70)
        {
            WalkSpeed = CrewData.WalkSpeed * 0.9f;
            RunSpeed = CrewData.RunSpeed * 0.9f;
            SitSpeed = CrewData.SitSpeed * 0.9f;
        }
        else if (Sanity < 30)
        {
            WalkSpeed = CrewData.WalkSpeed * 0.7f;
            RunSpeed = CrewData.RunSpeed * 0.7f;
            SitSpeed = CrewData.SitSpeed * 0.7f;
        }
    }

    #endregion
}
