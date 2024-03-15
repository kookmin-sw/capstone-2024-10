using Fusion;
using System;
using UnityEditor;
using UnityEngine;

public class PlayerSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnReadyCountChanged))]
    public int ReadyCount { get; set; }
    public Action OnReadyCountUpdate { get; set; }
    public void OnReadyCountChanged()
    {
        OnReadyCountUpdate.Invoke();
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.IsSharedModeMasterClient)
        {
            CountReady();
        }
    }

    public void CountReady()
    {
        int count = 0;
        foreach (var player in Runner.ActivePlayers)
        {
            NetworkObject po = Runner.GetPlayerObject(player);
            if (po == null)
                continue;

            if (po.GetComponent<Player>().State == Define.PlayerState.Ready)
            {
                count++;
            }
        }
        ReadyCount = count;
    }
}
