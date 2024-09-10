using UnityEngine;

public class SectorEntrance : MonoBehaviour
{
    [SerializeField] private Define.SectorName _sectorName;

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Creature creature))
        {
            if (Managers.ObjectMng.MyCreature != creature) return;

            if (creature.CurrentSector != _sectorName)
            {
                if (creature.CurrentSector != Define.SectorName.None) Managers.GameMng.MapSystem.Sectors[creature.CurrentSector].OnCreatureExit(creature);
                creature.CurrentSector = _sectorName;
                if (_sectorName != Define.SectorName.None) Managers.GameMng.MapSystem.Sectors[_sectorName].OnCreatureEnter(creature);
            }
        }
    }
}
