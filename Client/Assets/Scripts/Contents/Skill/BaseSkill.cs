using System.Collections;
using DG.Tweening;
using Fusion;
using UnityEngine;

public abstract class BaseSkill : NetworkBehaviour
{
    public string SkillDescription { get; set; }
    public float SkillTime { get; protected set; }
    public float CoolTime { get; protected set; }
    public float TotalSkillAmount { get; protected set; }
    public float CurrentSkillAmount { get; protected set; }
    public float AttackRange { get; protected set; }
    public bool Ready { get; protected set; }

    public Alien Owner { get; set; }

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        Owner = gameObject.GetComponent<Alien>();

        CurrentSkillAmount = 0f;
        Ready = true;
    }

    public abstract bool CheckAndUseSkill();

    public virtual void UseSkill()
    {
        Cooldown();

        Owner.CreatureState = Define.CreatureState.Use;
        Owner.CreaturePose = Define.CreaturePose.Stand;
    }

    public void ReadySkill()
    {
        Owner.IngameUI.WorkProgressBarUI.Show(SkillDescription.ToString(), CurrentSkillAmount, TotalSkillAmount);
        Owner.CreatureState = Define.CreatureState.Use;
        Owner.CreaturePose = Define.CreaturePose.Stand;

        Owner.AlienAnimController.PlayReadyRoar();

        StartCoroutine(CoReadySkill());
    }

    protected virtual IEnumerator CoReadySkill()
    {
        while (CurrentSkillAmount < TotalSkillAmount)
        {
            // if (Owner.CreatureState != Define.CreatureState.Use)
            //     SkillInterrupt();

            CurrentSkillAmount = Mathf.Clamp(CurrentSkillAmount + Time.deltaTime * Owner.CreatureData.WorkSpeed, 0, TotalSkillAmount);
            Owner.IngameUI.WorkProgressBarUI.CurrentWorkAmount = CurrentSkillAmount;

            yield return null;
        }

        UseSkill();
    }

    public void SkillInterrupt()
    {
        StopAllCoroutines();

        Owner.IngameUI.WorkProgressBarUI.Hide();

        CurrentSkillAmount = 0f;
    }

    public void Cooldown()
    {
        Ready = false;
        DOVirtual.DelayedCall(CoolTime, () => { Ready = true; });
    }
}

