using UnityEngine;

public class BasicAttack : BaseSkill
{
    protected override void Init()
    {
        base.Init();

        SkillDescription = "BASIC ATTACK";
        SkillTime = 1.5f;
        CoolTime = 2f;
        TotalSkillAmount = -1f;
        AttackRange = 1.5f;
    }

    public override bool CheckAndUseSkill()
    {
        if (!Ready)
            return false;

        Owner.CurrentSkillRange = AttackRange;
        UseSkill();
        return true;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Owner.AlienAnimController.PlayBasicAttack();

        Vector3 attackPosition = Owner.transform.position + Owner.CreatureCamera.transform.forward * AttackRange;

        Collider[] hitColliders = new Collider[4];
        int hitNum = Physics.OverlapSphereNonAlloc(attackPosition, AttackRange, hitColliders, LayerMask.GetMask("Crew"));
        if (hitNum > 0)
        {
            foreach (Collider col in hitColliders)
            {
                if (col != null && col.gameObject.TryGetComponent(out Crew crew))
                    crew.Rpc_OnDamaged(Owner.AlienStat.AttackDamage);
            }
        }

        SkillInterrupt();
        Owner.ReturnToIdle(SkillTime);
    }
}
