using System.Collections;
using Data;
using DG.Tweening;
using Fusion;
using UnityEngine;

public abstract class BaseSkill : NetworkBehaviour
{
    #region Field

    public int DataId { get; protected set; }
    public SkillData SkillData { get; protected set; }
    public Define.AlienActionType ReadySkillActionType { get; protected set; }
    public Define.AlienActionType SkillActionType { get; protected set; }

    public float CurrentSkillAmount { get; protected set; } = 0f;
    public float CurrentReadySkillAmount { get; protected set; } = 0f;
    public float CurrentCoolTime { get; set; } = 0f;
    public bool IsHit { get; protected set; } = false;

    public Alien Owner { get; protected set; }
    public Vector3 ForwardDirection => Owner.Transform.forward;
    public Vector3 AttackPosition { get; protected set; }

    private Tweener _coolTimeTweener;

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
    }

    public virtual bool CheckAndUseSkill()
    {
        if (CurrentCoolTime > 0f)
            return false;

        if (SkillData.Range > 0f)
            Owner.CurrentSkillRange = SkillData.Range;

        ReadySkill();
        return true;
    }

    public virtual void ReadySkill()
    {
        Owner.IngameUI.WorkProgressBarUI.Show(SkillData.Name, CurrentReadySkillAmount, SkillData.TotalReadySkillAmount);
        Owner.CreatureState = Define.CreatureState.Use;
        Owner.CreaturePose = Define.CreaturePose.Stand;

        PlayAnim(true);

        StartCoroutine(ReadySkillProgress());
    }

    public void UseSkill()
    {
        Cooldown();

        Owner.CreatureState = Define.CreatureState.Use;
        Owner.CreaturePose = Define.CreaturePose.Stand;

        StartCoroutine(ProgressSkill());
    }

    protected IEnumerator ReadySkillProgress()
    {
        while (CurrentReadySkillAmount < SkillData.TotalReadySkillAmount)
        {
            CurrentReadySkillAmount = Mathf.Clamp(CurrentReadySkillAmount + Time.deltaTime, 0, SkillData.TotalReadySkillAmount);
            Owner.IngameUI.WorkProgressBarUI.CurrentWorkAmount = CurrentReadySkillAmount;

            yield return null;
        }

        UseSkill();
    }

    protected virtual IEnumerator ProgressSkill()
    {
        yield return null;
    }

    protected void UpdateWorkAmount()
    {
        CurrentSkillAmount = Mathf.Clamp(CurrentSkillAmount + Time.deltaTime, 0, SkillData.TotalSkillAmount);
    }

    protected void Cooldown()
    {
        CurrentCoolTime = SkillData.CoolTime;
        _coolTimeTweener= DOVirtual.Float(0f, 0f, SkillData.CoolTime, value =>
        {
            CurrentCoolTime -= Time.deltaTime;
            if (CurrentCoolTime < 0.05f)
            {
                CurrentCoolTime = 0f;
                _coolTimeTweener.Kill();
            }
        });
    }

    protected virtual void DecideHit(float x, float y)
    {
        Ray ray = new Ray(AttackPosition + Owner.CameraRotationY * new Vector3(x, y, 0f), ForwardDirection);

        Debug.DrawRay(ray.origin, ray.direction * SkillData.Range, Color.red);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: SkillData.Range,
                layerMask: LayerMask.GetMask("Crew", "MapObject", "InteractableObject")))
        {
            if (rayHit.transform.gameObject.TryGetComponent(out Crew crew))
            {
                IsHit = true;
                Owner.AlienSoundController.PlaySound(Define.AlienActionType.Hit);
                crew.Rpc_OnDamaged(SkillData.Damage);
                crew.Rpc_OnSanityDamaged(SkillData.SanityDamage);
            }
        }
    }

    public virtual void SkillInterrupt(float hitDelayTime)
    {
        StopAllCoroutines();

        Owner.IngameUI.WorkProgressBarUI.Hide();

        CurrentSkillAmount = 0f;
        CurrentReadySkillAmount = 0f;

        if (IsHit)
        {
            HitDelay(hitDelayTime);
            return;
        }

        Owner.ReturnToIdle(0);
    }

    protected void HitDelay(float time)
    {
        if (time > 0f)
        {
            Owner.AlienAnimController.PlayAnim(Define.AlienActionType.HitDelay);
            Owner.AlienSoundController.PlaySound(Define.AlienActionType.HitDelay);
        }

        IsHit = false;
        Owner.ReturnToIdle(time);
    }

    protected void PlayAnim(bool isReady)
    {
        if (isReady)
            Owner.AlienAnimController.PlayAnim(ReadySkillActionType);
        else
            Owner.AlienAnimController.PlayAnim(SkillActionType);
    }

    protected void PlaySound()
    {
        Owner.AlienSoundController.PlaySound(SkillActionType);
    }
}

