using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class Sector : NetworkBehaviour
{
    [SerializeField]
    public Define.SectorName SectorName;

    [SerializeField]
    private Door[] _doors;
    private BaseWorkStation[] _workStations;
    private SectorLight _sectorLight;

    private List<Transform> _itemSpawnPoints = new();


    public void Init()
    {
        _workStations = GetComponentsInChildren<BaseWorkStation>();
        _itemSpawnPoints = gameObject.transform.FindObjectsWithTag("ItemSpawnPoint")
            .Select(obj => obj.transform)
            .ToList();
    }

    public bool SpawnItem(GameObject item)
    {
        if(_itemSpawnPoints.Count == 0) return false;

        // 스폰 포인트 랜덤 선택
        Transform spawnPoint = _itemSpawnPoints[Random.Range(0, _itemSpawnPoints.Count)];

        NetworkObject no = Runner.Spawn(item, spawnPoint.position);
        no.transform.SetParent(gameObject.transform);

        // 한 번 선택된 스폰포인트는 더이상 선택되지 않음
        _itemSpawnPoints.Remove(spawnPoint);

        return true;
    }
}
