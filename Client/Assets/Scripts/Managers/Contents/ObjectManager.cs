using System;
using System.Collections.Generic;
using Data;
using Fusion;
using UnityEngine;

public class ObjectManager
{
	public Dictionary<NetworkId, Crew> Crews { get; protected set; }
    public Dictionary<NetworkId, Alien> Aliens { get; protected set; }

    public Dictionary<int, BaseItem> Items { get; protected set; }

    public Creature MyCreature { get; set; }
    public Transform CrewRoot => GetRootTransform("@Crews");
    public Transform AlienRoot => GetRootTransform("@Aliens");

    public void Init()
    {
	    Crews = new Dictionary<NetworkId, Crew>();
	    Aliens = new Dictionary<NetworkId, Alien>();
        Items = new Dictionary<int, BaseItem>();

        BindItems();
    }

    public Transform GetRootTransform(string name)
    {
	    GameObject root = GameObject.Find(name);
	    if (root == null)
		    root = new GameObject { name = name };

	    return root.transform;
    }

    #region Bind

    public void BindItems()
    {
        foreach (var itemData in Managers.DataMng.ItemDataDict)
        {
            Type itemType = Type.GetType(itemData.Value.Name);
            if (itemType == null)
            {
                Debug.LogError("Failed to BindAction: " + itemData.Value.Name);
                return;
            }

            BaseItem item = (BaseItem)(Activator.CreateInstance(itemType));
            item.SetInfo(itemData.Key);

            Items[itemData.Key] = item;
        }
    }

    #endregion

    #region Creature

    public NetworkObject SpawnCrew(int crewDataId, Vector3 spawnPosition)
    {
        string className = Managers.DataMng.CrewDataDict[crewDataId].Name;
        NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"{Define.CREATURE_PATH}/{className}");
        NetworkObject no = Managers.NetworkMng.Runner.Spawn(prefab, spawnPosition);

        Crew crew = no.GetComponent<Crew>();
        crew.SetInfo(crewDataId);
        Player player = no.GetComponent<Player>();
        player.PlayerRef = Managers.NetworkMng.Runner.LocalPlayer;

        return no;
    }

    public NetworkObject SpawnAlien(int alienDataId, Vector3 spawnPosition)
    {
        string className = Managers.DataMng.AlienDataDict[alienDataId].Name;
        NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"{Define.CREATURE_PATH}/{className}");
        NetworkObject no = Managers.NetworkMng.Runner.Spawn(prefab, spawnPosition);

        Alien alien = no.GetComponent<Alien>();
        alien.SetInfo(alienDataId);
        Player player = no.GetComponent<Player>();
        player.PlayerRef = Managers.NetworkMng.Runner.LocalPlayer;

        return no;
    }

    public void Despawn(NetworkObject no)
    {
        Creature creature = no.GetComponent<Creature>();
        if (creature.CreatureType == Define.CreatureType.Crew)
            Crews.Remove(no.Id);
        else if (creature.CreatureType == Define.CreatureType.Alien)
            Aliens.Remove(no.Id);
        else
        {
            Debug.Log("Invalid Despawn");
            return;
        }
        if (creature == MyCreature)
            MyCreature = null;

        Managers.NetworkMng.Runner.Despawn(no);
    }

    public CreatureData GetCreatureDataWithDataId(int dataId)
    {
	    if (Managers.DataMng.CrewDataDict.TryGetValue(dataId, out CrewData crewData))
		    return crewData;
	    if (Managers.DataMng.AlienDataDict.TryGetValue(dataId, out AlienData alienData))
            return alienData;

        Debug.Log("Invalid GetCreatureDataWithDataId");
	    return null;
    }

    #endregion
}
