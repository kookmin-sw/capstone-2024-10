using System.Collections;
using Data;
using DG.Tweening;
using UnityEngine;

public class CrewStat : BaseStat
{
    CrewData CrewData => CreatureData as CrewData;

    public float SitSpeed { get; set; }
    public float RunSpeed { get; set; }
    public int Hp { get; set; }
    public int MaxHp { get; set; }
    public float Stamina { get; set; }
    public float MaxStamina { get; set; }
    public float Sanity { get; set; }
    public float MaxSanity { get; set; }
    public float RunUseStamina { get; set; }
    public float PassiveRecoverStamina { get; set; }
    public float DamagedRecoverStamina { get; set; }
    public float ErosionReduceSanity { get; set; }
    public float SitRecoverSanity { get; set; }

    public bool IsRunnable { get; set; } = true;
    public bool Exhausted { get; set; } = false;
    public bool Doped { get; set; } = false;
    public bool DamagedBoost { get; set; } = false;

    private Tween _damagedBoostTween;

    public override void SetStat(CreatureData creatureData)
    {
        base.SetStat(creatureData);

        SitSpeed = CrewData.SitSpeed;
        RunSpeed = CrewData.RunSpeed;
        Hp = CrewData.MaxHp;
        MaxHp = CrewData.MaxHp;
        Stamina = CrewData.MaxStamina;
        MaxStamina = CrewData.MaxStamina;
        Sanity = CrewData.MaxSanity;
        MaxSanity = CrewData.MaxSanity;
        RunUseStamina = CrewData.RunUseStamina;
        PassiveRecoverStamina = CrewData.PassiveRecoverStamina;
        DamagedRecoverStamina = CrewData.DamagedRecoverStamina;
        ErosionReduceSanity = CrewData.ErosionReduceSanity;
        SitRecoverSanity = CrewData.SitRecoverSanity;
    }

    #region Event

    public void ChangeHp(int value)
    {
        if (!HasStateAuthority)
            return;

        Hp = Mathf.Clamp(Hp + value, 0, MaxHp);

        if (value < 0)
        {
            Managers.GameMng.RenderingSystem.ApplyDamageEffect(Hp);

            if (Stamina < 60f)
                ChangeStamina(60f - Stamina);

            if (Sanity < 60f)
                ChangeSanity(60f - Sanity);

            DamagedBoost = true;

            _damagedBoostTween.Kill();
            _damagedBoostTween = DOVirtual.DelayedCall(5.5f, () =>
            {
                DamagedBoost = false;
            });
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
    }

    public void ChangeSanity(float value)
    {
        if (!HasStateAuthority)
            return;

        StartCoroutine(ProgressChangeSanity(value));
    }

    public IEnumerator ProgressChangeSanity(float value)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= 0.5f)
        {
            elapsedTime += Time.deltaTime;
            ApplySanity(value * 2f *Time.deltaTime);
            yield return null;
        }
    }

    public void ApplySanity(float value)
    {
        Sanity = Mathf.Clamp(Sanity + value, 0, MaxSanity);

        float ratio = 0.7f + Sanity * 0.003f;

        WalkSpeed = CrewData.WalkSpeed * ratio;
        RunSpeed = CrewData.RunSpeed * ratio;
        SitSpeed = CrewData.SitSpeed * ratio;

        Managers.GameMng.RenderingSystem.ApplySanityEffect(Sanity);
    }

    #endregion
}
