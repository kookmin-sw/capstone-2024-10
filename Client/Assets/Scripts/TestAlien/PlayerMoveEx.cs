using UnityEngine;
using UnityEngine.AI;

public class PlayerMoveEx : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    int RandomWanderRange = 50;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        WanderRandomly();
    }

    void Update()
    {
        // 목적지에 도달하면 새로운 랜덤 위치로 이동
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.1f)
        {
            WanderRandomly();
        }
    }

    void WanderRandomly()
    {
        // 랜덤한 위치를 생성하고 그 위치로 이동
        Vector3 randomPosition = GetRandomPosition();
        navMeshAgent.SetDestination(randomPosition);
    }

    Vector3 GetRandomPosition()
    {
        // NavMesh의 영역에서 랜덤한 위치를 얻음
        NavMeshHit hit;
        Vector3 randomPosition = transform.position + Random.insideUnitSphere * RandomWanderRange;
        NavMesh.SamplePosition(randomPosition, out hit, RandomWanderRange, NavMesh.AllAreas);

        return hit.position;
    }
}