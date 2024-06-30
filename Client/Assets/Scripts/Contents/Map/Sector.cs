using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class Sector : NetworkBehaviour
{
    [SerializeField] private Define.SectorName _sectorName;
    [SerializeField] private Define.SectorName[] _lightEnableTargetSectors;
    [SerializeField] private Light[] _additionalLightEnableTargets;
    public Define.SectorName SectorName => _sectorName;
    [Networked] public NetworkBool IsEroded { get; set; } = false;

    private List<Transform> _itemSpawnPoints = new();
    private Transform _itemParent;
    private List<Light> _lights = new();
    private bool _hasMyCreature;


    public void Init()
    {
        _itemSpawnPoints = gameObject.transform.FindObjectsWithTag("ItemSpawnPoint")
            .Select(obj => obj.transform)
            .ToList();
        _itemParent = GameObject.FindGameObjectWithTag("ItemParent").transform;

        _lights = transform.GetComponentsInChildren<Light>().ToList();
        if (_itemParent == null) _itemParent = Managers.GameMng.MapSystem.gameObject.transform;
    }

    public bool SpawnItem(GameObject go)
    {
        if(_itemSpawnPoints.Count == 0) return false;

        // 스폰 포인트 랜덤 선택
        Transform spawnPoint = _itemSpawnPoints[Random.Range(0, _itemSpawnPoints.Count)];

        NetworkObject no = Managers.NetworkMng.Runner.Spawn(go, spawnPoint.position, spawnPoint.rotation);
        no.transform.SetParent(_itemParent);

        BaseItemObject item = no.GetComponent<BaseItemObject>();
        if (item != null)
            item.SetInfo(true);

        // 한 번 선택된 스폰포인트는 더이상 선택되지 않음
        _itemSpawnPoints.Remove(spawnPoint);

        return true;
    }

    public void OnCreatureEnter(Creature creature)
    {
        _hasMyCreature = true;

        if (IsEroded)
            creature.Rpc_ApplyErosion(true);

        foreach (var sector in Managers.GameMng.MapSystem.Sectors.Keys)
        {
            if (_lightEnableTargetSectors.Contains(sector))
            {
                Managers.GameMng.MapSystem.Sectors[sector].EnableLight();
            }
            else
            {
                Managers.GameMng.MapSystem.Sectors[sector].DisableLight();
            }
        }

        foreach (var additionalLight in _additionalLightEnableTargets)
        {
            additionalLight.enabled = true;
        }
    }

    public void OnCreatureExit(Creature creature)
    {
        _hasMyCreature = false;

        creature.Rpc_ApplyErosion(false);
    }

    private void EnableLight()
    {
        foreach (var sectorlight in _lights)
        {
            sectorlight.enabled = true;
        }
    }

    private void DisableLight()
    {
        foreach (var sectorlight in _lights)
        {
            sectorlight.enabled = false;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_ApplyErosion()
    {
        IsEroded = true;

        Rpc_ApplyErosionAll();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_ApplyErosionAll()
    {
        if (_hasMyCreature)
            Managers.ObjectMng.MyCreature.Rpc_ApplyErosion(true);
    }
}
