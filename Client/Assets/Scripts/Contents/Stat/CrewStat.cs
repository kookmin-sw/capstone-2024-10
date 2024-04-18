using System;
using Data;
using UnityEngine;

public class CrewStat : BaseStat
{
    CrewData CrewData => CreatureData as CrewData;

    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public float Stamina { get; set; }
    public float MaxStamina { get; set; }
    public float Sanity { get; set; }
    public float MaxSanity { get; set; }
    public float SitSpeed { get; set; }

    public bool IsRunnable { get; set; }

    public Action<int> OnHpChanged;
    public Action<float> OnSanityChanged;

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

        if (Managers.MapMng.RenderingSystem)
        {
            OnSanityChanged += Managers.MapMng.RenderingSystem.SetChromaticAberration;
            OnSanityChanged += Managers.MapMng.RenderingSystem.SetVignette;
        }
    }

    #region Event

    public void ChangeHp(int value)
    {
        Hp = Mathf.Clamp(Hp + value, 0, MaxHp);
    }

    public void ChangeStamina(float value)
    {
        Stamina = Mathf.Clamp(Stamina + value, 0, MaxStamina);

        if (Stamina >= 20)
            IsRunnable = true;
        else if (Stamina <= 0)
            IsRunnable = false;
    }

    public void ChangeSanity(float value)
    {
        Sanity = Mathf.Clamp(Sanity + value, 0, MaxSanity);

        float ratio = 1f;
        if (Sanity >= 70)
            ratio = 1f;
        else if (Sanity < 70)
            ratio = 0.9f;
        else if (Sanity < 30)
            ratio = 0.7f;

        WalkSpeed = CrewData.WalkSpeed * ratio;
        RunSpeed = CrewData.RunSpeed * ratio;
        SitSpeed = CrewData.SitSpeed * ratio;

        OnSanityChanged?.Invoke(Sanity);
    }

    #endregion
}
