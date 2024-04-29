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
    public bool Exhausted { get; set; }
    public bool Doped { get; set; }

    public Action<int> OnHpChanged;
    public Action<float> OnSanityChanged;

    protected override void Init()
    {
        if (Managers.GameMng.RenderingSystem)
        {
            OnHpChanged += Managers.GameMng.RenderingSystem.DamageEffect;

            OnSanityChanged += Managers.GameMng.RenderingSystem.SetChromaticAberration;
            OnSanityChanged += Managers.GameMng.RenderingSystem.SetVignette;
        }
    }

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

    public void ChangeHp(int value)
    {
        if (!HasStateAuthority)
            return;

        Hp = Mathf.Clamp(Hp + value, 0, MaxHp);

        if (value < 0)
        {
            OnHpChanged.Invoke(Hp);
        }
    }

    public void ChangeStamina(float value)
    {
        if (!HasStateAuthority)
            return;

        Stamina = Mathf.Clamp(Stamina + value, 0, MaxStamina);

        if (Stamina >= 20)
            IsRunnable = true;
        else if (Stamina <= 0)
            IsRunnable = false;

        if (Stamina < 40 && !Exhausted)
        {
            Exhausted = true;
            Managers.SoundMng.Play($"{Define.EFFECT_PATH}/Crew/Exhaust", volume: 1f, isLoop: true);
        }
        if (Stamina >= 40)
        {
            if (Exhausted)
                Exhausted = false;
            Managers.SoundMng.Stop();
        }
    }

    public void ChangeSanity(float value)
    {
        if (!HasStateAuthority)
            return;

        Sanity = Mathf.Clamp(Sanity + value, 0, MaxSanity);

        float ratio = 1f;
        if (Sanity >= 70)
            ratio = 1f;
        else if (Sanity < 50)
            ratio = 0.85f;
        else if (Sanity < 25)
            ratio = 0.7f;

        WalkSpeed = CrewData.WalkSpeed * ratio;
        RunSpeed = CrewData.RunSpeed * ratio;
        SitSpeed = CrewData.SitSpeed * ratio;

        OnSanityChanged?.Invoke(Sanity);
    }

    #endregion
}
