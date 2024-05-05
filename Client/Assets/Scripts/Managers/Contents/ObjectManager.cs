using Fusion;
using UnityEngine;

public class ObjectManager
{
    public Creature MyCreature { get; set; }
    public Crew MyCrew => MyCreature as Crew;
    public Alien MyAlien => MyCreature as Alien;

    #region Creature

    public NetworkObject SpawnCrew(int crewDataId, Vector3 spawnPosition, bool isGameScene)
    {
        string className = Managers.DataMng.CrewDataDict[crewDataId].Name;
        NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"{Define.CREATURE_PATH}/{className}");
        NetworkObject no = Managers.NetworkMng.Runner.Spawn(prefab, spawnPosition);

        Crew crew = no.GetComponent<Crew>();
        crew.SetInfo(crewDataId, isGameScene);

        return no;
    }

    public NetworkObject SpawnAlien(int alienDataId, Vector3 spawnPosition)
    {
        string className = Managers.DataMng.AlienDataDict[alienDataId].Name;
        NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"{Define.CREATURE_PATH}/{className}");
        NetworkObject no = Managers.NetworkMng.Runner.Spawn(prefab, spawnPosition);

        Alien alien = no.GetComponent<Alien>();
        alien.SetInfo(alienDataId);

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
