using System;
using System.Collections.Generic;
using Data;
using Fusion;
using UnityEngine;

public class ObjectManager
{
	public Dictionary<NetworkId, Crew> Crews { get; protected set; }
    public Dictionary<NetworkId, Alien> Aliens { get; protected set; }

    public NetworkObject Player { get; protected set; }

    public void Init()
    {
	    Crews = new Dictionary<NetworkId, Crew>();
	    Aliens = new Dictionary<NetworkId, Alien>();
    }

    #region Roots
    public Transform GetRootTransform(string name)
    {
	    GameObject root = GameObject.Find(name);
	    if (root == null)
		    root = new GameObject { name = name };

	    return root.transform;
    }

    public Transform CrewRoot { get { return GetRootTransform("@Heroes"); } }
    public Transform AlienRoot { get { return GetRootTransform("@Monsters"); } }
    #endregion

    public NetworkObject SpawnCrew(int crewDataId)
    {
	    string className = Managers.DataMng.CrewDataDict[crewDataId].Name;
        NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"{Define.CREW_PATH}/{className}");
        NetworkObject no = Managers.NetworkMng.Runner.Spawn(prefab, Vector3.zero);
        Player = no;
        if (Camera.main != null)
            Camera.main.GetComponent<CameraController>().Player = Player.transform;

        Crew crew = no.GetComponent<Crew>();
        crew.SetInfo(crewDataId);

        return no;
    }

    public NetworkObject SpawnAlien(int alienDataId)
    {
        string className = Managers.DataMng.AlienDataDict[alienDataId].Name;
        NetworkObject prefab = Managers.ResourceMng.Load<NetworkObject>($"{Define.ALIEN_PATH}/{className}");
        NetworkObject no = Managers.NetworkMng.Runner.Spawn(prefab, Vector3.zero);

        Alien alien = no.GetComponent<Alien>();
        alien.SetInfo(alienDataId);

        return no;
    }

    public void Despawn(NetworkObject no)
    {
        Managers.NetworkMng.Runner.Despawn(no);
	}

    public CreatureData GetCreatureDataWithDataId(int dataId)
    {
	    CreatureData creatureData = null;
	    if (Managers.DataMng.CrewDataDict.TryGetValue(dataId, out CrewData crewData))
		    creatureData = crewData;
	    if (Managers.DataMng.AlienDataDict.TryGetValue(dataId, out AlienData alienData))
		    creatureData = alienData;

	    return creatureData;
    }
}
