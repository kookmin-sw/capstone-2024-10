using Fusion.Addons.SimpleKCC;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class LeapAttack : BaseSkill
{
    public SimpleKCC KCC { get; protected set; }
    private bool isMoving;
    private bool isMoveCoroutineRunning;

    protected override void Init()
    {
        base.Init();
        KCC = GetComponent<SimpleKCC>();

        SkillDescription = "LEAP ATTACK";
        SkillTime = 1f;//스킬 시전시간
        CoolTime = 4f;//재사용 대기시간
        TotalSkillAmount = 2f;//시전 대기시간
        AttackRange = 3f;
        isMoving = false;
        isMoveCoroutineRunning = false;
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

        Owner.AlienAnimController.PlayReadyLeapAttack();

        StartCoroutine(CoReadySkill());
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Owner.AlienAnimController.PlayLeapAttack();

        isMoving = true;

        SkillInterrupt();
        Owner.ReturnToIdle(SkillTime);

    }

    private IEnumerator MoveToTarget()
    {
        isMoveCoroutineRunning = true;

        //Debug.Log("move&");
        Vector3 forwardDirection = Owner.CreatureCamera.transform.forward;

        RaycastHit hit;
        if (Physics.SphereCast(Owner.transform.position, 2f, forwardDirection, out hit, 12f, LayerMask.GetMask("Crew")))
        {
            Vector3 targetPosition = hit.point; // 충돌 지점까지 이동할 위치
            float distance = Vector3.Distance(Owner.transform.position, targetPosition); // 충돌 지점까지의 거리

            float moveSpeed = distance / SkillTime * 4; // 이동 속도 계산, 4는 KCC.Movd에 맞춘 조정값.
            KCC.Move(forwardDirection * distance, moveSpeed); // 충돌 지점까지 이동

            // 이동에 걸리는 시간만큼 대기
            yield return new WaitForSeconds(SkillTime);

            // 이동이 끝난 후 공격 실행
            if (isMoveCoroutineRunning)
            {
                Attack();
            }
        }
        else
        {
            // 충돌이 감지되지 않았을 때 기본 값만큼 이동
            float moveSpeed = 4f; // 기본 이동 속도
            KCC.Move(forwardDirection * 12f, moveSpeed);

            yield return new WaitForSeconds(SkillTime);

            if (isMoveCoroutineRunning)
            {
                Attack();
            }
        }
        isMoving = false;
        isMoveCoroutineRunning = false;
    }


    private void Attack()
    {
        //isMoving = false;
        Debug.Log("hit");
        // 공격 로직
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
    }
    public override void FixedUpdateNetwork()
    {
        if (isMoving)
        {
            StartCoroutine(MoveToTarget());
        }
    }
}
