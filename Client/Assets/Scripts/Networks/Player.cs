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
        Managers.GameMng.Player = this;
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

        if (Managers.ObjectMng.MyCreature.CreatureType == Define.CreatureType.Crew)
        {
            if (creature.CreatureType == Define.CreatureType.Crew)
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
        OnPlayerNameUpdate?.Invoke((Managers.NetworkMng.Runner.IsSharedModeMasterClient ? " Master " : "") + PlayerName.Value);
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
    public static void RPC_SpawnPlayer(NetworkRunner runner, [RpcTarget] PlayerRef player, Vector3 spawnPos, bool isAlien)
    {
        NetworkObject no = isAlien
            ? Managers.ObjectMng.SpawnAlien(Define.ALIEN_STALKER_ID, spawnPos)
            : Managers.ObjectMng.SpawnCrew(Define.CREW_CREWA_ID, spawnPos);
        runner.SetPlayerObject(player, no);
    }
}
