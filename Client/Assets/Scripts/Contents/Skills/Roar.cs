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
            for (float i = -1.5f; i <= 1.5f && !IsHit; i += 0.2f)
            {
                for (float j = -1.5f; j <= 1.5f && !IsHit; j += 0.2f)
                {
                    Ray ray = new Ray(AttackPosition + Owner.CameraRotationY * new Vector3(i, j, 0f), ForwardDirection);

                    Debug.DrawRay(ray.origin, ray.direction * SkillData.Range, Color.red);

                    if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: SkillData.Range,
                            layerMask: LayerMask.GetMask("Crew", "MapObject", "PlanTargetObject")))
                    {
                        if (rayHit.transform.gameObject.TryGetComponent(out Crew crew))
                        {
                            IsHit = true;
                            crew.Rpc_OnDamaged(SkillData.Damage);
                            crew.Rpc_OnSanityDamaged(SkillData.SanityDamage);
                            Owner.SkillController.Skills[3].CurrentCoolTime -= 20f;
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
