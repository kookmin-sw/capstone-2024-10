using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class Sector : NetworkBehaviour
{
    [SerializeField] // 인스펙터 창에서 지정을 위해 필요
    private Define.SectorName _sectorName;
    public Define.SectorName SectorName => _sectorName;

    public List<Transform> ItemSpawnPoints { get; protected set; } =  new();
    public Transform ItemParent { get; protected set; }
    public bool MyCreatureIn { get; protected set; }

    [Networked] public NetworkBool IsErosion { get; set; } = false;

    public void Init()
    {
        ItemSpawnPoints = gameObject.transform.FindObjectsWithTag("ItemSpawnPoint")
            .Select(obj => obj.transform)
            .ToList();
        ItemParent = GameObject.FindGameObjectWithTag("ItemParent").transform;

        if (ItemParent == null) ItemParent = Managers.GameMng.MapSystem.gameObject.transform;
    }

    public bool SpawnItem(GameObject go)
    {
        if(ItemSpawnPoints.Count == 0) return false;

        // 스폰 포인트 랜덤 선택
        Transform spawnPoint = ItemSpawnPoints[Random.Range(0, ItemSpawnPoints.Count)];

        NetworkObject no = Managers.NetworkMng.Runner.Spawn(go, spawnPoint.position, spawnPoint.rotation);
        no.transform.SetParent(ItemParent);

        BaseItemObject item = no.GetComponent<BaseItemObject>();
        if (item != null)
            item.SetInfo(true);

        // 한 번 선택된 스폰포인트는 더이상 선택되지 않음
        ItemSpawnPoints.Remove(spawnPoint);

        return true;
    }

    public void OnCreatureEnter(Creature creature)
    {
        MyCreatureIn = true;

        if (IsErosion)
            creature.Rpc_SetErosion(true);

        Debug.Log($"{SectorName}: OnCreatureEnter");
    }

    public void OnCreatureExit(Creature creature)
    {
        MyCreatureIn = false;

        creature.Rpc_SetErosion(false);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_GetErosion()
    {
        IsErosion = true;

        Rpc_GetErosionAll();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_GetErosionAll()
    {
        if (MyCreatureIn)
            Managers.ObjectMng.MyCreature.Rpc_SetErosion(true);
    }
}
