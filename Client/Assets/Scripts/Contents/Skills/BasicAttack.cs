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
        if (!Ready)
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

        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            if (CurrentSkillAmount > 0.3f)
            {
                Vector3 attackPosition = Owner.transform.position + ForwardDirection * SkillData.Range;
                Collider[] hitColliders = new Collider[3];

                if (!IsHit && Physics.OverlapSphereNonAlloc(attackPosition, SkillData.Range, hitColliders,
                        LayerMask.GetMask("Crew")) > 0)
                {
                    if (hitColliders[0].gameObject.TryGetComponent(out Crew crew))
                    {
                        IsHit = true;
                        Owner.AlienSoundController.PlaySound(Define.AlienActionType.Hit);
                        crew.Rpc_OnDamaged(SkillData.Damage);
                        crew.Rpc_OnSanityDamaged(SkillData.SanityDamage);
                    }
                }
            }

            UpdateWorkAmount();
            yield return null;
        }

        SkillInterrupt();
    }
}
