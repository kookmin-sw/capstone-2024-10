using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class Sector : MonoBehaviour
{
    [SerializeField] // 인스펙터 창에서 지정을 위해 필요
    private Define.SectorName _sectorName;
    public Define.SectorName SectorName => _sectorName;

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
        Debug.Log($"{SectorName}: OnCreatureEnter");
        if (creature is Crew)
        {
            // TODO: 전역효과 효과 적용 등
        }
    }

    public void OnCreatureExit(Creature creature)
    {
        Debug.Log($"{SectorName}: OnCreatureExit");
        if (creature is Crew)
        {
            // TODO: 전역효과 효과 해제 등
        }
    }
}
