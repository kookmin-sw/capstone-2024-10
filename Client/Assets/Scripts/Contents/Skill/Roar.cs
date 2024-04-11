using System.Collections;
using UnityEngine;

public class Roar : BaseSkill
{
    protected override void Init()
    {
        base.Init();

        SkillDescription = "ROAR";
        CoolTime = 4f;
        TotalSkillAmount = 2.1f;
        TotalReadySkillAmount = 1f;
        AttackRange = 3f;
    }

    public override void ReadySkill()
    {
        Owner.IngameUI.WorkProgressBarUI.Show(SkillDescription, CurrentReadySkillAmount, TotalReadySkillAmount);
        Owner.CreatureState = Define.CreatureState.Use;
        Owner.CreaturePose = Define.CreaturePose.Stand;

        Owner.AlienAnimController.PlayReadyRoar();

        StartCoroutine(ReadySkillProgress());
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Owner.AlienAnimController.PlayRoar();

        StartCoroutine(ProgressSkill());
    }

    protected override IEnumerator ProgressSkill()
    {

        while (CurrentSkillAmount < TotalSkillAmount)
        {
            Vector3 attackPosition = Owner.transform.position + ForwardDirection * AttackRange;
            Collider[] hitColliders = new Collider[3];

            if (!IsHit && Physics.OverlapSphereNonAlloc(attackPosition, AttackRange, hitColliders, LayerMask.GetMask("Crew")) > 0)
            {
                if (hitColliders[0].gameObject.TryGetComponent(out Crew crew))
                {
                    IsHit = true;
                    crew.Rpc_OnSanityDamaged(Owner.AlienStat.RoarSanityDamage);
                }
            }

            UpdateWorkAmount(Time.deltaTime);
            yield return null;
        }

        SkillInterrupt();
    }
}
