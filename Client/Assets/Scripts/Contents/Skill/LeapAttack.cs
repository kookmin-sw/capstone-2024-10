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

        IsMoving = false;
    }

    public override void ReadySkill()
    {
        Owner.IngameUI.WorkProgressBarUI.Show(SkillData.Name, CurrentReadySkillAmount, SkillData.TotalReadySkillAmount);
        Owner.CreatureState = Define.CreatureState.Use;
        Owner.CreaturePose = Define.CreaturePose.Stand;

        Owner.AlienAnimController.PlayReadyLeapAttack();

        StartCoroutine(ReadySkillProgress());
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Owner.AlienAnimController.PlayLeapAttack();

        StartCoroutine(ProgressSkill());
    }

    protected override IEnumerator ProgressSkill()
    {
        IsMoving = true;

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

            UpdateWorkAmount(Time.deltaTime);
            yield return null;
        }

        IsMoving = false;
        SkillInterrupt();
    }

    public override void FixedUpdateNetwork()
    {
        if (IsMoving)
            KCC.Move(ForwardDirection * 5, 0);
    }
}
