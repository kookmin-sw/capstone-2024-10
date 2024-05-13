using Data;
using DG.Tweening;
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

    public bool IsRunnable { get; set; } = true;
    public bool Exhausted { get; set; } = false;
    public bool Doped { get; set; } = false;
    public bool DamagedBoost { get; set; } = false;

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
    }

    #region Event

    public void ChangeHp(int value)
    {
        if (!HasStateAuthority)
            return;

        Hp = Mathf.Clamp(Hp + value, 0, MaxHp);

        if (value < 0)
        {
            Managers.GameMng.RenderingSystem.DamageEffect(Hp);

            DamagedBoost = true;
            DOVirtual.DelayedCall(3.5f, () =>
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

        float ratio = 0.75f + Sanity * 0.003f;

        WalkSpeed = CrewData.WalkSpeed * ratio;
        RunSpeed = CrewData.RunSpeed * ratio;
        SitSpeed = CrewData.SitSpeed * ratio;
        WorkSpeed = CrewData.WorkSpeed * ratio;

        Managers.GameMng.RenderingSystem.ApplySanity(Sanity);
    }

    #endregion
}
