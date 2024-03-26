using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class Player : NetworkBehaviour
{
    [Networked] public NetworkString<_32> PlayerName { get; set; }

    [Networked]
    public PlayerRef PlayerRef { get; set; }
    public Action OnPlayerNameUpdate { get; set; }

    public Creature Creature { get; set; }
    [Networked]
    public Define.PlayerState State { get; set; } = Define.PlayerState.None;

    public override void Spawned()
    {
        if (!HasStateAuthority)
            return;

        PlayerName = Managers.NetworkMng.PlayerName;
        Managers.GameMng.Player = this;
    }

    private void Update()
    {
        if (HasStateAuthority && Runner.IsSharedModeMasterClient)
        {
            PlayerName = "Master";
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isActiveAndEnabled);

        var creature = GetComponent<Creature>();
        if (creature.CreatureType == Define.CreatureType.Crew)
        {
            var ui = Managers.UIMng.MakeWorldSpaceUI<UI_NameTag>(transform);

            if (PlayerRef == Runner.LocalPlayer)
            {
                ui.gameObject.SetActive(false);
            }
        }

        yield return new WaitUntil(() => Object != null && Object.IsValid);

        OnPlayerNameUpdate?.Invoke();
    }

    public void GetReady()
    {
        if (State == Define.PlayerState.None)
        {
            State = Define.PlayerState.Ready;
        }
    }

    public async void ExtiGame()
    {
        await Runner.Shutdown();
        Managers.SceneMng.LoadScene(Define.SceneType.LobbyScene);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public static async void RPC_ChangePlayerToAlien(NetworkRunner runner, [RpcTarget] PlayerRef player, int alienDataId)
    {
        NetworkObject po;
        while (!runner.TryGetPlayerObject(player, out po))
        {
            await Task.Delay(100);
        }
        Managers.ObjectMng.Despawn(po);
        Vector3 spawnPosition = po.transform.position;
        NetworkObject no = Managers.ObjectMng.SpawnAlien(alienDataId, spawnPosition);
        runner.SetPlayerObject(player, no);
    }
}
