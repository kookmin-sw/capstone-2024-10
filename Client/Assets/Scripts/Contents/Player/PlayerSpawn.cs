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
            // 랜덤한 인덱스를 선택합니다.
            int randomIndex = Random.Range(0, PlayerPrefab.Length);

            Runner.Spawn(PlayerPrefab[randomIndex], new Vector3(0, 0.137f, 0), Quaternion.identity);
        }
    }
}
