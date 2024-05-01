using System.Collections;
using UnityEngine;

public class Roar : BaseSkill
{
    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);

        ReadySkillActionType = Define.AlienActionType.ReadyRoar;
        SkillActionType = Define.AlienActionType.Roar;
    }

    protected override IEnumerator ProgressSkill()
    {
        PlayAnim(false);
        PlaySound();

        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            Collider[] hitColliders = new Collider[3];

            if (!IsHit && Physics.OverlapSphereNonAlloc(AttackPosition, SkillData.Range, hitColliders, LayerMask.GetMask("Crew")) > 0)
            {
                if (hitColliders[0].gameObject.TryGetComponent(out Crew crew))
                {
                    IsHit = true;
                    crew.Rpc_OnSanityDamaged(SkillData.SanityDamage);
                }
            }

            UpdateWorkAmount();
            yield return null;
        }

        SkillInterrupt();
    }
}
