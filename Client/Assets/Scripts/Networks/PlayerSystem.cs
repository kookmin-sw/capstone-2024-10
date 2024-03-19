using Fusion;
using System;
using UnityEditor;
using UnityEngine;

public class PlayerSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnReadyCountChanged))]
    public int ReadyCount { get; set; }
    public Action OnReadyCountUpdated { get; set; }
    public void OnReadyCountChanged()
    {
        OnReadyCountUpdated.Invoke();
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.IsSharedModeMasterClient)
        {
            CountReady();
            // 종료 로직 나중에 추가
        }
    }

    public Player GetPlayer()
    {
        if (Runner.TryGetPlayerObject(Runner.LocalPlayer, out NetworkObject player))
            return player.GetComponent<Player>();

        return null;
    }

    public void CountReady()
    {
        int count = 0;
        foreach (var player in Runner.ActivePlayers)
        {
            NetworkObject po = Runner.GetPlayerObject(player);
            if (po == null)
                continue;

            Player p = po.GetComponent<Player>();
            if (p == null)
                continue;

            if (p.State == Define.PlayerState.Ready)
            {
                count++;
            }
        }
        ReadyCount = count;
    }
}
