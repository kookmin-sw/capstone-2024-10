using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class MapSystem : NetworkBehaviour
{
    public Dictionary<Define.SectorName, Sector> Sectors { get; set; } = new();

    public void Init()
    {
        Managers.GameMng.MapSystem = this;
        AssignSector();
        GetComponent<ItemSpawner>()?.SpawnItems();
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

        Sectors[Managers.ObjectMng.MyCreature.CurrentSector].OnCreatureEnter(Managers.ObjectMng.MyCreature);
    }
}
