using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectManager
{
    public Creature MyCreature { get; set; }
    public Crew MyCrew => MyCreature as Crew;
    public Alien MyAlien => MyCreature as Alien;

    #region Creature

    public async Task<NetworkObject> SpawnCrew(int crewDataId, Vector3 spawnPos, Define.SectorName spawnSector, bool isGameScene)
    {
        string className = Managers.DataMng.CrewDataDict[crewDataId].Name;
        NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"{Define.CREATURE_PATH}/{className}");
        NetworkObject no = await Managers.NetworkMng.Runner.SpawnAsync(prefab, spawnPos);

        Crew crew = no.GetComponent<Crew>();
        crew.SetInfo(crewDataId, isGameScene);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        crew.CurrentSector = spawnSector;

        return no;
    }

    public async Task<NetworkObject> SpawnAlien(int alienDataId, Vector3 spawnPos, Define.SectorName spawnSector)
    {
        string className = Managers.DataMng.AlienDataDict[alienDataId].Name;
        NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"{Define.CREATURE_PATH}/{className}");
        NetworkObject no = await Managers.NetworkMng.Runner.SpawnAsync(prefab, spawnPos);

        Alien alien = no.GetComponent<Alien>();
        alien.SetInfo(alienDataId);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        alien.CurrentSector = spawnSector;
        return no;
    }

    public NetworkObject SpawnItemObject(int itemDataId, Vector3 spawnPosition, bool isGettable)
    {
        string className = Managers.DataMng.ItemDataDict[itemDataId].Name;
        NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"{Define.ITEM_OBJECT_PATH}/{className}");
        NetworkObject no = Managers.NetworkMng.Runner.Spawn(prefab, spawnPosition);

        BaseItemObject item = no.GetComponent<BaseItemObject>();
        item.SetInfo(isGettable);

        return no;
    }

    #endregion
}
