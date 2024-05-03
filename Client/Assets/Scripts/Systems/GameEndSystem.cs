using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndSystem : NetworkBehaviour
{

    public float DeathCount { get; set; }

    public void Init()
    {
        DeathCount = 0;
    }
    public void EndGame()
    {
        if (Managers.NetworkMng.NumPlayers == 1)
        {

            if (DeathCount <= 3)
            {
                Managers.ObjectMng.MyAlien.OnAllKill();
            }
            else
            {
                Managers.ObjectMng.MyAlien.OnClear();
            }
        }
    }
}
