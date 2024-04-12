using Fusion.Addons.SimpleKCC;
using System.Collections;
using UnityEngine;

public class LeapAttack : BaseSkill
{
    public SimpleKCC KCC => Owner.KCC;

    public bool IsMoving { get; protected set; }

    protected override void Init()
    {
        base.Init();

        SkillDescription = "LEAP ATTACK";
        CoolTime = 4f;
        TotalSkillAmount = 2f;
        TotalReadySkillAmount = 1f;
        AttackRange = 1.5f;

        IsMoving = false;
    }

    public override void ReadySkill()
    {
        Owner.IngameUI.WorkProgressBarUI.Show(SkillDescription, CurrentReadySkillAmount, TotalReadySkillAmount);
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

        while (CurrentSkillAmount < TotalSkillAmount)
        {
            if (CurrentSkillAmount < TotalSkillAmount - 0.5f)
            {
                Vector3 attackPosition = Owner.transform.position + ForwardDirection * AttackRange;
                Collider[] hitColliders = new Collider[3];

                if (!IsHit && Physics.OverlapSphereNonAlloc(attackPosition, AttackRange, hitColliders, LayerMask.GetMask("Crew")) > 0)
                {
                    if (hitColliders[0].gameObject.TryGetComponent(out Crew crew))
                    {
                        IsHit = true;
                        IsMoving = false;
                        crew.Rpc_OnDamaged(Owner.AlienStat.AttackDamage);
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
