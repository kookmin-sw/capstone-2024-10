using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class Sector : MonoBehaviour
{
    [SerializeField]
    public Define.SectorName sectorName;

    private List<Transform> _itemSpawnPoints = new();
    private Transform _itemParent;

    public void Init()
    {
        _itemSpawnPoints = gameObject.transform.FindObjectsWithTag("ItemSpawnPoint")
            .Select(obj => obj.transform)
            .ToList();
        _itemParent = GameObject.FindGameObjectWithTag("ItemParent").transform;

        if (_itemParent == null) _itemParent = Managers.GameMng.MapSystem.gameObject.transform;
    }

    public bool SpawnItem(GameObject go)
    {
        if(_itemSpawnPoints.Count == 0) return false;

        // 스폰 포인트 랜덤 선택
        Transform spawnPoint = _itemSpawnPoints[Random.Range(0, _itemSpawnPoints.Count)];

        NetworkObject no = Managers.NetworkMng.Runner.Spawn(go, spawnPoint.position, spawnPoint.rotation);
        //no.transform.SetParent(Managers.GameMng.MapSystem.gameObject.transform);
        no.transform.SetParent(_itemParent);

        BaseItemObject item = no.GetComponent<BaseItemObject>();
        if (item != null)
            item.SetInfo(true);

        // 한 번 선택된 스폰포인트는 더이상 선택되지 않음
        _itemSpawnPoints.Remove(spawnPoint);

        return true;
    }
}
