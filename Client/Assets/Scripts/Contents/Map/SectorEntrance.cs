using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectorEntrance : MonoBehaviour
{
    [SerializeField] private Define.SectorName _sectorName;

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Creature>(out var creature))
        {
            if (creature.CurrentSector != _sectorName)
            {
                if (creature.CurrentSector != Define.SectorName.None) Managers.GameMng.MapSystem.Sectors[creature.CurrentSector].OnCreatureExit(creature);
                creature.CurrentSector = _sectorName;
                if (_sectorName != Define.SectorName.None) Managers.GameMng.MapSystem.Sectors[_sectorName].OnCreatureEnter(creature);
            }
        }
    }
}
