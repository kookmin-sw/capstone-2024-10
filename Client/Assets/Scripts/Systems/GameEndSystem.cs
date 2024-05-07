using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndSystem : NetworkBehaviour
{
    [Networked] public NetworkBool IsCrewClear { get; set; } = false;
    [Networked] public NetworkBool IsGameEnd { get; set; } = false;
    
    public void Init()
    {
        Managers.GameMng.GameEndSystem = this;
    }

    public void EndGame()
    {
        if (IsGameEnd)
        {
            if (Managers.NetworkMng.NumPlayers <= 1)
            {
                if (IsCrewClear)
                {
                    Managers.ObjectMng.MyAlien.OnClear();
                    IsGameEnd = false;
                }
                else
                {
                    Managers.ObjectMng.MyAlien.OnAllKill();
                    IsGameEnd = false;
                }
            }
        }
        else
        {
            return;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_GameClear()
    {
        IsCrewClear = true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_GameEnd()
    {
        IsGameEnd = true;
    }

    public async void Exit()
    {
        await Runner.Shutdown();
        Managers.SceneMng.LoadScene(Define.SceneType.LobbyScene);
    }
}
