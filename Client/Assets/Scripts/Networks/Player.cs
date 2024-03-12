using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;

public class Player : NetworkBehaviour
{
    [Networked] public NetworkString<_32> PlayerName { get => default; set { } }
    [Networked, OnChangedRender(nameof(OnReadyCountChanged))]
    public int ReadyCount { get; set; }

    public Action OnPlayerNameUpdate { get; set; }
    public Action OnReadyCountUpdate { get; set; }
    public Define.PlayerState State { get; protected set; } = Define.PlayerState.None;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            PlayerName = Managers.NetworkMng.PlayerName;
        }

        Managers.GameMng.Player = this;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isActiveAndEnabled);

        Managers.UIMng.MakeWorldSpaceUI<UI_NameTag>(transform);

        yield return new WaitUntil(() => PlayerName.Value != null);

        OnPlayerNameUpdate.Invoke();
    }

    public void OnReadyCountChanged()
    {

    }

    public void GetReady()
    {
        if (State == Define.PlayerState.None)
        {
            ReadyCount++;
            State = Define.PlayerState.Ready;
        }
    }
}
