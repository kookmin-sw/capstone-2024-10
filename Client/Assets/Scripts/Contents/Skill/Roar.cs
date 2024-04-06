using UnityEngine;

public class Roar : BaseSkill
{
    protected override void Init()
    {
        base.Init();

        SkillDescription = "ROAR";
        SkillTime = 2.1f;
        CoolTime = 4f;
        TotalSkillAmount = 3f;
        AttackRange = 3f;
    }

    public override bool CheckAndUseSkill()
    {
        if (!Ready)
            return false;

        Owner.CurrentSkillRange = AttackRange;
        ReadySkill();
        return true;
    }

    public override void ReadySkill()
    {
        Owner.IngameUI.WorkProgressBarUI.Show(SkillDescription, CurrentSkillAmount, TotalSkillAmount);
        Owner.CreatureState = Define.CreatureState.Use;
        Owner.CreaturePose = Define.CreaturePose.Stand;

        Owner.AlienAnimController.PlayReadyRoar();

        StartCoroutine(CoReadySkill());
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Owner.AlienAnimController.PlayRoar();

        Vector3 attackPosition = Owner.transform.position + Owner.CreatureCamera.transform.forward * AttackRange;

        Collider[] hitColliders = new Collider[4];
        int hitNum = Physics.OverlapSphereNonAlloc(attackPosition, AttackRange, hitColliders, LayerMask.GetMask("Crew"));
        if (hitNum > 0)
        {
            foreach (Collider col in hitColliders)
            {
                if (col != null && col.gameObject.TryGetComponent(out Crew crew))
                    crew.Rpc_OnSanityDamaged(Owner.AlienStat.RoarSanityDamage);
            }
        }

        SkillInterrupt();
        Owner.ReturnToIdle(SkillTime);
    }
}
