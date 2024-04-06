using Fusion;
using UnityEngine;

public class PlayerSpawn : SimulationBehaviour, IPlayerJoined
{
    public GameObject[] PlayerPrefab;
    public int PrefabNum;
    public Vector3 PlayerSpawnPosition;
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            NetworkObject no = Runner.Spawn(PlayerPrefab[PrefabNum], PlayerSpawnPosition, Quaternion.identity);
            Creature creature = no.GetComponent<Creature>();
            if (creature is Crew)
            {
                creature.GetComponent<Crew>().SetInfo(Define.CREW_CREWA_ID);
            }
            else
            {
                creature.GetComponent<Alien>().SetInfo(Define.ALIEN_STALKER_ID);
            }

            // if (PrefabNum == 0)
            //     Managers.ObjectMng.SpawnCrew(Define.CREW_CREWA_ID, PlayerSpawnPosition);
            // else if (PrefabNum == 1)
            //     Managers.ObjectMng.SpawnAlien(Define.ALIEN_STALKER_ID, PlayerSpawnPosition);
        }
    }
}
