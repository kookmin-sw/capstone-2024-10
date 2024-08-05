using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class Sector : NetworkBehaviour
{
    [SerializeField] private Define.SectorName _sectorName;
    [SerializeField] private Define.SectorName[] _lightEnableTargetSectors; // 섹터에 진입 시, 빛을 활성화할 섹터 목록
    [SerializeField] private Light[] _additionalLightEnableTargets; // 섹터에 진입 시, 활성화할 빛 목록 (특정 섹터 전체의 빛을 활성화한다면 _lightEnableTargetSectors 사용)
    public Define.SectorName SectorName => _sectorName;
    [Networked] public NetworkBool IsEroded { get; set; } = false;

    private List<Transform> _itemSpawnPoints = new();
    private Transform _itemParent;

    private List<SectorLight> _lights = new();
    private List<SectorLight> _enableTargetLights = new();

    private bool _hasMyCreature;

    public void Init()
    {
        _itemSpawnPoints = gameObject.transform.FindObjectsWithTag("ItemSpawnPoint")
            .Select(obj => obj.transform)
            .ToList();
        _itemParent = GameObject.FindGameObjectWithTag("ItemParent").transform;

        // 섹터의 모든 빛에 SectorLight 부착
        var lights = transform.GetComponentsInChildren<Light>().ToList();
        foreach (var l in lights)
        {
            var sl = l.gameObject.GetOrAddComponent<SectorLight>();
            sl.Init();
            _lights.Add(sl);
        }

        foreach (var l in _additionalLightEnableTargets)
        {
            var sl = l.gameObject.GetOrAddComponent<SectorLight>();
            sl.Init();
            _enableTargetLights.Add(sl);
        }

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

        // 주변 섹터의 빛만 활성화
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

        foreach (var additionalLight in _enableTargetLights)
        {
            additionalLight.EnableLight();
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
            sectorlight.EnableLight();
        }
    }

    private void DisableLight()
    {
        foreach (var sectorlight in _lights)
        {
            sectorlight.DisableLight();
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
