using Fusion;
using System;
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

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_InformReady(bool ready = true)
    {
        if (ready)
        {
            ReadyCount++;
        }
        else
        {
            ReadyCount--;
        }
    }
}
