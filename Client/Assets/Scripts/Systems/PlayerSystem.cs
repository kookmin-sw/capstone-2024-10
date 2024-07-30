using Fusion;
using System;
using UnityEngine;

public class PlayerSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnReadyCountChanged))]
    public int ReadyCount { get; set; }
    public Action OnReadyCountUpdated { get; set; }

    [Networked, Capacity(Define.PLAYER_COUNT)]
    public NetworkDictionary<PlayerRef, SpawnPoint.SpawnPointData> SpawnPoints { get; }
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
