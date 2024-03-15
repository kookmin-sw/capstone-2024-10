using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerSpawn : SimulationBehaviour, IPlayerJoined
{
    public GameObject[] PlayerPrefab;
    public Vector3 PlayerSpawnPosition;
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            NetworkObject no = Runner.Spawn(PlayerPrefab[0], PlayerSpawnPosition, Quaternion.identity);
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
