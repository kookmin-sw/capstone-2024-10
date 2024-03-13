using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using ExitGames.Client.Photon;

public class Player : NetworkBehaviour
{
    [Networked] public NetworkString<_32> PlayerName { get => default; set { } }

    public Action OnPlayerNameUpdate { get; set; }
    public Define.PlayerState State { get; protected set; } = Define.PlayerState.None;

    public override void Spawned()
    {
        if (!HasStateAuthority)
            return;

        PlayerName = Managers.NetworkMng.PlayerName;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isActiveAndEnabled);

        Managers.GameMng.Player = this;
        Managers.UIMng.MakeWorldSpaceUI<UI_NameTag>(transform);

        yield return new WaitUntil(() => PlayerName.Value != null);

        OnPlayerNameUpdate.Invoke();
    }

    public void GetReady()
    {
        if (State == Define.PlayerState.None)
        {
            Managers.NetworkMng.PlayerSystem.RPC_InformReady(true);
            State = Define.PlayerState.Ready;
        }
    }

    public void ExtiGame()
    {
        Runner.Shutdown();
        Managers.SceneMng.LoadScene(Define.SceneType.LobbyScene);
    }
}
