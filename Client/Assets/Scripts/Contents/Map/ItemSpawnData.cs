using System;
using UnityEngine;

[Serializable]
public struct ItemSpawnData
{
    public GameObject Prefab;
    public int GlobalMinCount;
    public int GlobalMaxCount;
    public int MaxCountPerSector;
    [Tooltip("해당 아이템을 스폰하지 않을 섹터")] public Define.SectorName[] ExcludedSectors;

    public bool Validate()
    {
        if (GlobalMaxCount < GlobalMinCount)
        {
            Debug.LogError($"{Prefab.name} data: MaxCount < MinCount!");
            return false;
        }

        if (GlobalMaxCount < MaxCountPerSector)
        {
            Debug.LogError($"{Prefab.name} data: MaxCount < MaxCountPerSector!");
            return false;
        }
        return true;
    }
}
