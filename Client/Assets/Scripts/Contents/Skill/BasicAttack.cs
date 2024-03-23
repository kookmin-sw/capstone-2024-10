using Fusion;
using UnityEngine;

public class BasicAttack : BaseSkill
{
    private float attackRange = 2f; // 근접 공격 범위

    public override bool CheckAndUseSkill()
    {
        Rpc_UseSkill();
        return true;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_UseSkill()
    {
        try
        {
            // 주인공 캐릭터의 위치와 방향을 기준으로 공격 범위 내의 대상을 찾음
            Collider[] hitColliders = Physics.OverlapSphere(Owner.transform.position, attackRange);

            // 찾은 대상에 대해 각각 데미지를 입히는 동작 수행
            foreach (Collider col in hitColliders)
            {
                // 대상이 공격자 자신이 아니고, 대상이 Crew 클래스를 가진 경우
                if (col.gameObject != Owner.gameObject && col.gameObject.TryGetComponent(out Crew crew))
                {
                    // 대상에게 데미지를 입히는 함수 호출
                    crew.OnDamaged(Owner.AlienStat.Damage);
                    Debug.Log(crew.DataId + " take damage");
                }
            }
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log("Attack Miss : NullReferenceException caught: " + ex.Message);
            // 예외 처리
        }
    }
}
