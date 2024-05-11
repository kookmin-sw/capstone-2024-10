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

        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            if (CurrentSkillAmount > 0.3f && !IsHit)
            {
                for (float i = -1f; i <= 1f && !IsHit; i += 0.2f)
                {
                    for (float j = -1f; j <= 1f && !IsHit; j += 0.2f)
                    {
                        Ray ray = new Ray(AttackPosition + Owner.CameraRotationY * new Vector3(i, j, 0f), ForwardDirection);

                        Debug.DrawRay(ray.origin, ray.direction * SkillData.Range, Color.red);

                        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: SkillData.Range,
                                layerMask: LayerMask.GetMask("Crew", "MapObject", "PlanTargetObject")))
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
                }
            }

            UpdateWorkAmount();
            yield return null;
        }

        SkillInterrupt();
    }
}
