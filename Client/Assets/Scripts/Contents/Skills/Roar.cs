using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Roar : BaseSkill
{
    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);

        ReadySkillActionType = Define.AlienActionType.ReadyRoar;
        SkillActionType = Define.AlienActionType.Roar;
    }

    public override void ReadySkill()
    {
        base.ReadySkill();

        DOVirtual.Float(0, SkillData.Range * 0.6f, SkillData.TotalReadySkillAmount, value =>
        {
            Owner.RoarRangeIndicator.transform.localScale = new Vector3(1f, 1.6f, value);
        });
    }

    protected override IEnumerator ProgressSkill()
    {
        PlayAnim(false);
        PlaySound();

        AttackPosition = Owner.Transform.position + Vector3.up * 1.5f;
        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            for (float i = -SkillData.Width; i <= SkillData.Width && !IsHit; i += 0.2f)
            {
                for (float j = -SkillData.Height; j <= SkillData.Height && !IsHit; j += 0.2f)
                {
                    DecideHit(i, j);
                }
            }

            UpdateWorkAmount();
            yield return null;
        }

        SkillInterrupt(0f);
    }

    public override void SkillInterrupt(float hitDelayTime)
    {
        base.SkillInterrupt(hitDelayTime);

        Owner.RoarRangeIndicator.transform.localScale = new Vector3(0f, 0f, 0f);
    }
}
