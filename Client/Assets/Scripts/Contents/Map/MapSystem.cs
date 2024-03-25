using System;
using System.Collections;
using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MapSystem : SimulationBehaviour
{
    [Header("Item Spawn")]
    [SerializeField] private int _totalItemCount;
    [SerializeField] private List<ItemSpawnData> _itemSpawnDatas;

    public Dictionary<Define.SectorName, Sector> Sectors { get; set; } = new();
    
    public void Init()
    {
        this.RegisterRunner();
        AssignSector();
        SpawnItems();
    }

    private void AssignSector()
    {
        Sector[] sectors = FindObjectsOfType<Sector>();

        foreach (Sector s in sectors)
        {
            s.Init();
            if (!Sectors.TryAdd(s.SectorName, s))
            {
                Debug.LogError($"Duplicate sector name detected!: {s.SectorName}");
            }
        }
    }

    private void SpawnItems()
    {
        if(!Runner.IsSharedModeMasterClient) return;

        int totalCount = 0;
        Dictionary<ItemSpawnData, int> count = new();
        List<Define.SectorName> availableSectors = new(Sectors.Keys);
        List<ItemSpawnData> itemSpawnData = new(_itemSpawnDatas);
        foreach (var key in itemSpawnData)
        {
            count.Add(key, 0);
        }

        // 아이템별 최소 개수 충족시키기
        foreach (var data in itemSpawnData)
        {
            // 데이터 검증도 동시에 함
            if (!data.Validate())
            {
                itemSpawnData.Remove(data);
                continue;
            }

            int cnt = 0;

            while (cnt < data.GlobalMinCount)
            {
                if(!TrySpawnItem(data)) return;
                cnt++;
            }
        }

        // 총 아이템 수에 맞춰 아이템 배치
        while (itemSpawnData.Count != 0 && totalCount < _totalItemCount)
        {
            var selected = itemSpawnData[Random.Range(0, itemSpawnData.Count)];
            if (!TrySpawnItem(selected)) return;
        }

        return;

        bool TrySpawnItem(ItemSpawnData data)
        {
            Define.SectorName selectedSector = availableSectors[Random.Range(0, availableSectors.Count)];
            while (!Sectors[selectedSector].SpawnItem(data.Prefab))
            {
                availableSectors.Remove(selectedSector);
                if (availableSectors.Count == 0)
                {
                    Debug.LogError("Cannot spawn more items: no more available sectors");
                    return false;
                }
                selectedSector = availableSectors[Random.Range(0, availableSectors.Count)];
            }

            count[data]++;
            totalCount++;
            if (data.GlobalMaxCount == count[data]) itemSpawnData.Remove(data);
            return true;
        }
    }
}
