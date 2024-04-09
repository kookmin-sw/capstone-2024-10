using System;
using System.Collections;
using Fusion;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class MapSystem : NetworkBehaviour
{
    public Dictionary<Define.SectorName, Sector> Sectors { get; set; } = new();

    [Header("Item Spawn")]
    [SerializeField] private int _totalItemCount;
    [SerializeField] private List<ItemSpawnData> _itemSpawnDatas;

    [Networked, OnChangedRender(nameof(OnBatteryCharge))] public int BatteryChargeCount { get; set; }
    public bool IsBatteryChargeFinished { get; set; }
    [Networked, OnChangedRender(nameof(OnGeneratorRestored))] public bool IsGeneratorRestored { get; set; }
    
    public void Init()
    {
        AssignSector();
        SpawnItems();
        Managers.MapMng.MapSystem = this;
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
        Dictionary<ItemSpawnData, int> totalItemCount = new();
        Dictionary<Define.SectorName, Dictionary<ItemSpawnData, int>> itemPerSectorCount = new();
        List<Define.SectorName> availableSectors = new(Sectors.Keys);
        List<ItemSpawnData> itemSpawnData = new(_itemSpawnDatas);

        foreach (var key in itemSpawnData)
        {
            totalItemCount.Add(key, 0);
        }

        foreach (var sector in availableSectors)
        {
            Dictionary<ItemSpawnData, int> itemSpawnDataCount = new();
            foreach (var data in itemSpawnData)
            {
                itemSpawnDataCount[data] = 0;
            }
            itemPerSectorCount.Add(sector, itemSpawnDataCount);
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
            if(!TrySelectSector(data, out Define.SectorName selectedSector)) return false;
            
            while (!Sectors[selectedSector].SpawnItem(data.Prefab))
            {
                availableSectors.Remove(selectedSector);
                if (availableSectors.Count == 0)
                {
                    Debug.LogWarning("Cannot spawn more items: no more available sectors");
                    return false;
                }
                if (!TrySelectSector(data, out selectedSector)) return false;
            }

            totalItemCount[data]++;
            itemPerSectorCount[selectedSector][data]++;
            totalCount++;
            if (data.GlobalMaxCount == totalItemCount[data]) itemSpawnData.Remove(data);
            return true;
        }

        bool TrySelectSector(ItemSpawnData data, out Define.SectorName selectedSector)
        {
            List<Define.SectorName> availableSectorsCopy = new(availableSectors);

            selectedSector = Define.SectorName.MainRoom;

            // Item의 섹터 당 개수 제한을 고려하여 섹터 선택
            while (availableSectorsCopy.Count > 0)
            {
                selectedSector = availableSectorsCopy[Random.Range(0, availableSectorsCopy.Count)];
                if (itemPerSectorCount[selectedSector][data] < data.MaxCountPerSector)
                {
                    return true;
                }

                availableSectorsCopy.Remove(selectedSector);
            }
            Debug.LogWarning($"{data.Prefab.name}: Could not select sector!");
            return false;
        }
    }

    private void OnBatteryCharge()
    {
        Managers.ObjectMng.MyCrew.CrewIngameUI.ObjectiveUI.UpdateBatteryCount(BatteryChargeCount);

        if (BatteryChargeCount == Define.BATTERY_COLLECT_GOAL)
        {
            IsBatteryChargeFinished = true;
        }
    }

    private void OnGeneratorRestored()
    {
        Managers.ObjectMng.MyCrew.CrewIngameUI.ObjectiveUI.OnGeneratorRestored();
    }
}
