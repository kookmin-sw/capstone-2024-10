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
            if (CurrentSkillAmount < SkillData.TotalSkillAmount - 0.5f)
            {
                Vector3 attackPosition = Owner.transform.position + ForwardDirection * SkillData.Range;
                Collider[] hitColliders = new Collider[3];

                if (!IsHit && Physics.OverlapSphereNonAlloc(attackPosition, SkillData.Range, hitColliders, LayerMask.GetMask("Crew")) > 0)
                {
                    if (hitColliders[0].gameObject.TryGetComponent(out Crew crew))
                    {
                        IsHit = true;
                        IsMoving = false;
                        crew.Rpc_OnDamaged(SkillData.Damage);
                        crew.Rpc_OnSanityDamaged(SkillData.SanityDamage);
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
