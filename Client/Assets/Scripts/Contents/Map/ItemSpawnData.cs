using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ItemSpawnData
{
    public GameObject Prefab;
    public int GlobalMinCount;
    public int GlobalMaxCount;
    public int MaxCountPerSector;

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
