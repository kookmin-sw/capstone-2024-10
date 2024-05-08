using Fusion.Addons.SimpleKCC;
using System.Collections;
using UnityEngine;

public class LeapAttack : BaseSkill
{
    public SimpleKCC KCC => Owner.KCC;

    public bool IsMoving { get; protected set; }

    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);

        ReadySkillActionType = Define.AlienActionType.ReadyLeapAttack;
        SkillActionType = Define.AlienActionType.LeapAttack;

        IsMoving = false;
    }

    protected override IEnumerator ProgressSkill()
    {
        IsMoving = true;

        PlayAnim(false);
        PlaySound();

        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            if (CurrentSkillAmount < SkillData.TotalSkillAmount - 0.5f && !IsHit)
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
            else
                IsMoving = false;

            UpdateWorkAmount();
            yield return null;
        }

        IsMoving = false;
        SkillInterrupt();
    }

    public override void FixedUpdateNetwork()
    {
        if (IsMoving)
            KCC.Move(ForwardDirection * (150f * Runner.DeltaTime), 0);
    }
}
