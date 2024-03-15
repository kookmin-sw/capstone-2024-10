using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSpawn : SimulationBehaviour, IPlayerJoined
{
    public GameObject[] PlayerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            NetworkObject no = Runner.Spawn(PlayerPrefab[0], new Vector3(0, 0.137f, 0), Quaternion.identity);
            Creature creature = no.GetComponent<Creature>();
            if (creature is Crew)
            {
                creature.GetComponent<Crew>().SetInfo(Define.CREW_CREWA_ID);
            }
            else
            {
                creature.GetComponent<Alien>().SetInfo(Define.ALIEN_STALKER_ID);
            }
        }
    }
}
