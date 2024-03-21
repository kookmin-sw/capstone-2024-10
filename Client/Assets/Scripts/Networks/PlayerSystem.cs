using Fusion;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnReadyCountChanged))]
    public int ReadyCount { get; set; }
    public Action OnReadyCountUpdated { get; set; }
    public void OnReadyCountChanged()
    {
        OnReadyCountUpdated?.Invoke();
    }

    public override void Spawned()
    {
        if (Runner.IsSharedModeMasterClient)
        {
            if (Managers.SceneMng.CurrentScene.SceneType == Define.SceneType.ReadyScene)
                StartCoroutine(CountReady());
        }
    }

    public override void FixedUpdateNetwork()
    {
    }

    public Player GetPlayer()
    {
        if (Runner.TryGetPlayerObject(Runner.LocalPlayer, out NetworkObject player))
            return player.GetComponent<Player>();

        return null;
    }

    public IEnumerator CountReady()
    {
        while (ReadyCount != Define.PLAYER_COUNT)
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

            yield return null;
        }
    }

    public void PlayerJoined()
    {
        Vector3 position = Vector3.zero;
        GameObject spawnPoint = GameObject.FindWithTag("Respawn");
        if (spawnPoint != null)
        {
            position = spawnPoint.transform.position;
        }

        NetworkObject playerObject = Managers.ObjectMng.SpawnCrew(Define.CREW_CREWA_ID, position);
        Runner.SetPlayerObject(Runner.LocalPlayer, playerObject);
    }
}
