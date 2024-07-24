using Fusion.Addons.SimpleKCC;
using System.Collections;
using UnityEngine;

public class LeapAttack : BaseSkill
{
    public SimpleKCC KCC => Owner.KCC;

    public bool IsMoving { get; protected set; } = false;
    public bool IsCurrentSectorEroded => Owner.CurrentSector != Define.SectorName.None&& Managers.GameMng.MapSystem.Sectors[Owner.CurrentSector].IsEroded;

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

        if (!IsCurrentSectorEroded)
            return false;

        if (SkillData.Range > 0f)
            Owner.CurrentSkillRange = SkillData.Range;

        ReadySkill();
        return true;
    }

    protected override IEnumerator ProgressSkill()
    {
        PlayAnim(false);
        PlaySound();

        IsMoving = true;

        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            if (CurrentSkillAmount < SkillData.TotalSkillAmount - 0.5f && !IsHit)
            {
                AttackPosition = Owner.Head.transform.position + Vector3.down * 0.2f;
                for (float i = -1f; i <= 1f && !IsHit; i += 0.2f)
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
        SkillInterrupt(3f);
    }

    public override void FixedUpdateNetwork()
    {
        if (IsMoving)
        {
            KCC.Move(ForwardDirection * (180f * Runner.DeltaTime), 0);
        }
    }
}
