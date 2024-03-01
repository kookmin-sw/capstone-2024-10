using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNetworkMovement : NetworkBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator anim;
    int IdleOne;
    int IdleAlert;
    int Sleeps;
    int AngryReaction;
    int Hit;
    int AnkleBite;
    int CrochBite;
    int Dies;
    int HushLittleBaby;
    int Run;

    int RandomWanderRange = 50;
    float detectionRange = 25f; // 플레이어 감지 범위

    public override void Spawned()
    {
        anim = GetComponent<Animator>();
        IdleOne = Animator.StringToHash("IdleOne");
        IdleAlert = Animator.StringToHash("IdleAlert");
        Sleeps = Animator.StringToHash("Sleeps");
        AngryReaction = Animator.StringToHash("AngryReaction");
        Hit = Animator.StringToHash("Hit");
        AnkleBite = Animator.StringToHash("AnkleBite");
        CrochBite = Animator.StringToHash("CrochBite");
        Dies = Animator.StringToHash("Dies");
        HushLittleBaby = Animator.StringToHash("HushLittleBaby");
        Hit = Animator.StringToHash("Hit");
        Run = Animator.StringToHash("Run");

        navMeshAgent = GetComponent<NavMeshAgent>();
        WanderRandomly();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority == false)
        {
            return;
        }

        base.FixedUpdateNetwork();
        // 플레이어 감지 및 추적 로직 추가
        DetectAndChasePlayer();

        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 감지
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 클릭한 위치로 NavMeshAgent 이동 목적지 설정
                navMeshAgent.SetDestination(hit.point);
            }
        }

        // 목적지에 도달하면 새로운 랜덤 위치로 이동
        else if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.1f)
        {
            WanderRandomly();
        }

        // 랜덤 배회 중에 Run 애니메이션 실행
        if (navMeshAgent.velocity.magnitude > 0.1f)
        {
            anim.SetBool(Run, true);
        }
        else
        {
            anim.SetBool(Run, false);
        }
    }

    void WanderRandomly()
    {
        // 랜덤한 위치를 생성하고 그 위치로 이동
        Vector3 randomPosition = GetRandomPosition();
        navMeshAgent.SetDestination(randomPosition);
    }

    void DetectAndChasePlayer()
    {
        // 주변에 플레이어가 있는지 감지
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                // 플레이어가 감지되면 추적 시작
                ChasePlayer(hitCollider.transform.position);
                return;
            }
        }
    }

    void ChasePlayer(Vector3 playerPosition)
    {
        // 플레이어를 추적하는 함수 호출
        navMeshAgent.SetDestination(playerPosition);
    }

    Vector3 GetRandomPosition()
    {
        // NavMesh의 영역에서 랜덤한 위치를 얻음
        NavMeshHit hit;
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * RandomWanderRange;//탐색 반지름
        NavMesh.SamplePosition(randomPosition, out hit, RandomWanderRange, NavMesh.AllAreas);//최대시도횟수

        return hit.position;
    }
}
