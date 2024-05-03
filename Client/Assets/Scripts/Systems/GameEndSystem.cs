using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndSystem : NetworkBehaviour
{

    public float DeathCount = 0.0f;

    public void Init()
    {
        Managers.GameMng.GameEndSystem = this;
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
