using Fusion;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectManager
{
    public Creature MyCreature { get; set; }
    public Crew MyCrew => MyCreature as Crew;
    public Alien MyAlien => MyCreature as Alien;

    #region Creature

    public async Task<NetworkObject> SpawnCrew(int crewDataId, SpawnPoint.SpawnPointData spawnPoint, bool isGameScene, bool isTutorial = false)
    {
        string className = Managers.DataMng.CrewDataDict[crewDataId].Name;
        NetworkObject prefab = null;
        NetworkObject no = null;

        if(isTutorial)
        {
            prefab = Managers.ResourceMng.Load<NetworkObject>($"Prefabs/Creatures/CrewT");
            no = await Managers.NetworkMng.Runner.SpawnAsync(prefab, spawnPoint.Position);
        }
        else
        {
            prefab = Managers.ResourceMng.Load<NetworkObject>($"{Define.CREATURE_PATH}/{className}");
            no = await Managers.NetworkMng.Runner.SpawnAsync(prefab, spawnPoint.Position);
        }

        Crew crew = no.GetComponent<Crew>();
        crew.SetInfo(crewDataId, isGameScene);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        crew.CurrentSector = spawnPoint.SectorName;

        return no;
    }

    public async Task<NetworkObject> SpawnAlien(int alienDataId, SpawnPoint.SpawnPointData spawnPoint, bool isTutorial = false)
    {
        string className = Managers.DataMng.AlienDataDict[alienDataId].Name;
        NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"{Define.CREATURE_PATH}/{className}");
        NetworkObject no = await Managers.NetworkMng.Runner.SpawnAsync(prefab, spawnPoint.Position);

        Alien alien = no.GetComponent<Alien>();
        alien.SetInfo(alienDataId);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        alien.CurrentSector = spawnPoint.SectorName;
        return no;
    }

    public NetworkObject SpawnItemObject(int itemDataId, Vector3 spawnPosition, Quaternion spawnRotation, bool isGettable)
    {
        string className = Managers.DataMng.ItemDataDict[itemDataId].Name;
        NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"{Define.ITEM_OBJECT_PATH}/{className}");
        NetworkObject no = Managers.NetworkMng.Runner.Spawn(prefab, spawnPosition, spawnRotation);

        BaseItemObject item = no.GetComponent<BaseItemObject>();
        item.SetInfo(isGettable);

        return no;
    }

    #endregion
}
