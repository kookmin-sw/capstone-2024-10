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

        AttackPosition = Owner.Head.transform.position + Vector3.down * 0.2f;
        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            for (float i = -2f; i <= 2f && !IsHit; i += 0.2f)
            {
                for (float j = -1f; j <= 1f && !IsHit; j += 0.2f)
                {
                    DecideHit(i, j);
                }
            }

            UpdateWorkAmount();
            yield return null;
        }

        SkillInterrupt(0f);
    }

    protected override void DecideHit(float x, float y)
    {
        Ray ray = new Ray(AttackPosition + Owner.CameraRotationY * new Vector3(x, y, 0f), ForwardDirection);

        Debug.DrawRay(ray.origin, ray.direction * SkillData.Range, Color.red);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: SkillData.Range,
                layerMask: LayerMask.GetMask("Crew", "MapObject", "InteractableObject")))
        {
            if (rayHit.transform.gameObject.TryGetComponent(out Crew crew))
            {
                IsHit = true;
                crew.Rpc_OnDamaged(SkillData.Damage);
                crew.Rpc_OnSanityDamaged(SkillData.SanityDamage);
                Owner.SkillController.Skills[2].CurrentCoolTime -= 20f;
            }
        }
    }

    public override void SkillInterrupt(float hitDelayTime)
    {
        base.SkillInterrupt(hitDelayTime);

        Owner.RoarRangeIndicator.transform.localScale = new Vector3(1f, 1.6f, 0f);
    }
}
