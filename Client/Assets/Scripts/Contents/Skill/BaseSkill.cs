using System.Collections;
using Data;
using DG.Tweening;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BaseSkill : NetworkBehaviour
{
    #region Field

    public int DataId { get; protected set; }
    public SkillData SkillData { get; protected set; }

    public float CurrentSkillAmount { get; protected set; }
    public float CurrentReadySkillAmount { get; protected set; }
    public bool Ready { get; protected set; }
    public bool IsHit { get; protected set; }

    public Alien Owner { get; protected set; }
    public Vector3 ForwardDirection => Owner.transform.forward;

    #endregion

    public override void Spawned()
    {
        Init();
    }

    protected void Init()
    {
        Owner = gameObject.GetComponent<Alien>();
    }

    public virtual void SetInfo(int templateId)
    {
        DataId = templateId;
        SkillData = Managers.DataMng.SkillDataDict[templateId];

        CurrentSkillAmount = 0f;
        CurrentReadySkillAmount = 0f;
        Ready = true;
        IsHit = false;
    }

    public virtual bool CheckAndUseSkill()
    {
        if (!Ready)
            return false;

        if (SkillData.Range > 0f)
            Owner.CurrentSkillRange = SkillData.Range;

        ReadySkill();
        return true;
    }

    public virtual void ReadySkill() { }

    public virtual void UseSkill()
    {
        Cooldown();

        Owner.CreatureState = Define.CreatureState.Use;
        Owner.CreaturePose = Define.CreaturePose.Stand;
    }

    protected void UpdateWorkAmount(float deltaTime)
    {
        CurrentSkillAmount = Mathf.Clamp(CurrentSkillAmount + deltaTime, 0, SkillData.TotalSkillAmount);
    }

    protected void Cooldown()
    {
        Ready = false;
        DOVirtual.DelayedCall(SkillData.CoolTime, () => { Ready = true; });
    }

    public void SkillInterrupt()
    {
        StopAllCoroutines();

        Owner.IngameUI.WorkProgressBarUI.Hide();

        CurrentSkillAmount = 0f;
        CurrentReadySkillAmount = 0f;

        IsHit = false;

        Owner.ReturnToIdle(0);
    }

    protected IEnumerator ReadySkillProgress()
    {
        while (CurrentReadySkillAmount < SkillData.TotalReadySkillAmount)
        {
            // if (Owner.CreatureState != Define.CreatureState.Use)
            //     SkillInterrupt();

            CurrentReadySkillAmount = Mathf.Clamp(CurrentReadySkillAmount + Time.deltaTime * Owner.CreatureData.WorkSpeed, 0, SkillData.TotalReadySkillAmount);
            Owner.IngameUI.WorkProgressBarUI.CurrentWorkAmount = CurrentReadySkillAmount;

            yield return null;
        }

        UseSkill();
    }

    protected virtual IEnumerator ProgressSkill()
    {
        yield return null;
    }
}

