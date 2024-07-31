using System.Collections;
using UnityEngine;

public class BasicAttack : BaseSkill
{
    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);

        ReadySkillActionType = Define.AlienActionType.None;
        SkillActionType = Define.AlienActionType.BasicAttack;
    }

    public override bool CheckAndUseSkill()
    {
        if (CurrentCoolTime > 0f)
            return false;

        if (SkillData.Range > 0f)
            Owner.CurrentSkillRange = SkillData.Range;

        UseSkill();
        return true;
    }

    protected override IEnumerator ProgressSkill()
    {
        PlayAnim(false);
        PlaySound();

        AttackPosition = Owner.Head.transform.position + Vector3.down * 0.2f;
        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            if (CurrentSkillAmount > 0.3f && CurrentSkillAmount < 0.6f && !IsHit)
            {
                for (float i = -SkillData.Width; i <= SkillData.Width && !IsHit; i += 0.2f)
                {
                    for (float j = -SkillData.Height; j <= SkillData.Height && !IsHit; j += 0.2f)
                    {
                        DecideHit(i, j);
                    }
                }
            }

            UpdateWorkAmount();
            yield return null;
        }

        SkillInterrupt(3f);
    }
}
