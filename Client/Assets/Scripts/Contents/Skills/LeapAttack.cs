using Fusion.Addons.SimpleKCC;
using System.Collections;
using UnityEngine;

public class LeapAttack : BaseSkill
{
    public SimpleKCC KCC => Owner.KCC;

    public bool IsMoving { get; protected set; } = false;
    public bool IsErosion => Managers.GameMng.MapSystem.Sectors[Owner.CurrentSector].IsErosion;

    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);

        ReadySkillActionType = Define.AlienActionType.ReadyLeapAttack;
        SkillActionType = Define.AlienActionType.LeapAttack;
    }

    public override bool CheckAndUseSkill()
    {
        if (CurrentCoolTime > 0f)
            return false;

        if (!IsErosion)
            return false;

        if (SkillData.Range > 0f)
            Owner.CurrentSkillRange = SkillData.Range;

        ReadySkill();
        return true;
    }

    protected override IEnumerator ProgressSkill()
    {
        IsMoving = true;

        PlayAnim(false);
        PlaySound();

        AttackPosition = Owner.Head.transform.position + Vector3.down * 0.2f;
        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            if (CurrentSkillAmount < SkillData.TotalSkillAmount - 0.5f && !IsHit)
            {
                for (float i = -0.5f; i <= 0.5f && !IsHit; i += 0.2f)
                {
                    for (float j = -1f; j <= 1f && !IsHit; j += 0.2f)
                    {
                        DecideHit(i, j);
                    }
                }
            }
            else
                IsMoving = false;

            UpdateWorkAmount();
            yield return null;
        }

        IsMoving = false;
        SkillInterrupt(1.5f);
    }

    public override void FixedUpdateNetwork()
    {
        if (IsMoving)
            KCC.Move(ForwardDirection * (150f * Runner.DeltaTime), 0);
    }
}
