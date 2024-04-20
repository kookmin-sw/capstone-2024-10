using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using UnityEngine.UIElements;

public class Player : NetworkBehaviour
{
    [Networked] public NetworkString<_32> PlayerName { get; set; }
    [Networked]
    NetworkBool IsMaster { get; set; }

    [Networked]
    public PlayerRef PlayerRef { get; set; }
    public Action<string> OnPlayerNameUpdate { get; set; }

    public Creature Creature { get; set; }
    [Networked]
    public Define.PlayerState State { get; set; } = Define.PlayerState.None;

    public override void Spawned()
    {
        if (!HasStateAuthority)
            return;

        PlayerRef = Runner.LocalPlayer;
        Managers.StartMng.Player = this;
        PlayerName = Managers.NetworkMng.PlayerName;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isActiveAndEnabled);

        yield return new WaitUntil(() => Runner.IsRunning);

        Define.SceneType sceneType = Managers.SceneMng.CurrentScene.SceneType;
        if (sceneType != Define.SceneType.GameScene && sceneType != Define.SceneType.ReadyScene)
            yield break;

        var creature = GetComponent<Creature>();

        if (Managers.ObjectMng.MyCreature is Crew)
        {
            if (creature is Crew)
            {
                var ui = Managers.UIMng.MakeWorldSpaceUI<UI_NameTag>(transform);

                if (PlayerRef == Runner.LocalPlayer)
                {
                    ui.gameObject.SetActive(false);
                }
            }
        }
    }

    private void Update()
    {
        OnPlayerNameUpdate?.Invoke((IsMaster ? " Master " : "") + PlayerName.Value);

        if (HasStateAuthority)
        {
            IsMaster = Runner.IsSharedModeMasterClient;
        }
    }

    public void GetReady()
    {
        if (State == Define.PlayerState.None)
        {
            State = Define.PlayerState.Ready;
        }
    }

    public async void ExitGame()
    {
        await Runner.Shutdown();
        Managers.SceneMng.LoadScene(Define.SceneType.LobbyScene);
    }

    [Rpc]
    public static async void RPC_SpawnPlayer(NetworkRunner runner, [RpcTarget] PlayerRef player, Vector3 spawnPos, bool isAlien)
    {
        await Task.Delay(500);

        NetworkObject no = isAlien
            ? Managers.ObjectMng.SpawnAlien(Define.ALIEN_STALKER_ID, spawnPos)
            : Managers.ObjectMng.SpawnCrew(Define.CREW_CREWA_ID, spawnPos, true);
        runner.SetPlayerObject(player, no);
    }
}
