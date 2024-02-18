using System;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class ObjectManager
{
	public Dictionary<ulong, Crew> Crews { get; protected set; }
    public Dictionary<ulong, Alien> Aliens { get; protected set; }

    public ulong NextCrewId;
    public ulong NextAlienId;

    public void Init()
    {
	    Crews = new Dictionary<ulong, Crew>();
	    Aliens = new Dictionary<ulong, Alien>();
	    NextCrewId = 10000;
	    NextAlienId = 20000;
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

    public Crew SpawnHero(int crewDataId)
    {
	    string className = Managers.DataMng.CrewDataDict[crewDataId].Name;
		GameObject go = Managers.ResourceMng.Instantiate($"{Define.CREW_PATH}/{className}");
		Crew crew = go.GetComponent<Crew>();

		//crew.SetInfo(crewDataId);
		go.transform.position = Vector3.zero;
		crew.transform.parent = CrewRoot;
		//crew.Id = NextCrewId;
		Crews[NextCrewId++] = crew;

		return crew;
	}

    public Alien SpawnMonster(int alienDataId)
    {
	    string className = Managers.DataMng.AlienDataDict[alienDataId].Name;
	    GameObject go = Managers.ResourceMng.Instantiate($"{Define.ALIEN_PATH}/{className}");
	    Alien alien = go.GetComponent<Alien>();

	    //alien.SetInfo(alienDataId);
	    go.transform.position = Vector3.zero;
	    alien.transform.parent = AlienRoot;
	    //alien.Id = NextAlienId;
	    Aliens[NextAlienId++] = alien;

	    return alien;
    }

    public void Despawn(Define.CreatureType creatureType, ulong id)
    {
	    Creature creature = null;
		switch (creatureType)
		{
			case Define.CreatureType.Crew:
				creature = Crews[id];
				Crews.Remove(id);
				break;
			case Define.CreatureType.Monster:
				creature = Crews[id];
				Aliens.Remove(id);
				break;
		}

		if (creature != null)
			Managers.ResourceMng.Destroy(creature.gameObject);
	}

    public Creature GetCreatureWithId(ulong id)
    {
	    Creature creature = null;
	    if (Managers.ObjectMng.Crews.TryGetValue(id, out Crew crew))
		    creature = crew;
	    if (Managers.ObjectMng.Aliens.TryGetValue(id, out Alien alien))
		    creature = alien;

	    return creature;
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
