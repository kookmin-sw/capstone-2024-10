using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnReadyCountChanged))]
    public int ReadyCount { get; set; }
    public Action OnReadyCountUpdated { get; set; }

    [Networked, Capacity(Define.PLAYER_COUNT)]
    public NetworkDictionary<PlayerRef, Vector3> SpawnPoints { get; }

    public enum PlayState
    {
        None,
        Ready,
        Game,
        Transition,
    }

    [Networked, OnChangedRender(nameof(OnPlayStateChanged))]
    public PlayState CurrentPlayState { get; set; }

    public override void Spawned()
    {
        DontDestroyOnLoad(gameObject);
        CurrentPlayState = PlayState.Ready;
    }

    public void OnReadyCountChanged()
    {
        OnReadyCountUpdated?.Invoke();
    }

    public void OnPlayStateChanged()
    {
        Debug.Log($"Current Play State: {CurrentPlayState}");
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority)
            return;

        if (Managers.SceneMng.CurrentScene == null)
            return;

        if (Managers.SceneMng.CurrentScene.SceneType == Define.SceneType.ReadyScene)
            CountReady();
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
