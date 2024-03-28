using UnityEngine;

public class BasicAttack : BaseSkill
{
    private float attackRange = 2f; // 근접 공격 범위

    public override bool CheckAndUseSkill()
    {
        if (!Ready)
            return false;

        UseSkill();
        return true;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Owner.AlienAnimController.PlayBasicAttack();

        Vector3 attackPosition = Owner.transform.position + Owner.CreatureCamera.transform.forward * attackRange;

        Collider[] hitColliders = new Collider[4];
        int hitNum = Physics.OverlapSphereNonAlloc(attackPosition, attackRange, hitColliders, LayerMask.GetMask("Crew"));
        if (hitNum > 0)
        {
            foreach (Collider col in hitColliders)
            {
                if (col != null && col.gameObject.TryGetComponent(out Crew crew))
                    crew.Rpc_OnDamaged(Owner.AlienStat.Damage);
            }
        }

        Owner.ReturnToIdle(SkillTime);
    }
}
