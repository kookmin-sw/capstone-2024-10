using System.Collections;
using UnityEngine;
using Fusion;
using System;
using System.Linq;

public class Player : NetworkBehaviour
{
    [Networked] public NetworkString<_32> PlayerName { get; set; }
    [Networked]
    public NetworkBool IsMaster { get; set; }

    [Networked]
    public PlayerRef PlayerRef { get; set; }
    public Action<string> OnPlayerNameUpdate { get; set; }

    public Creature Creature { get; private set; } 
    public Define.CreatureType CreatureType { get; private set; } = Define.CreatureType.None;
    [Networked]
    public Define.PlayerState State { get; set; } = Define.PlayerState.None;
    [Networked] public bool IsSpawned { get; set; }
    [Networked] public bool AreAllPlayersSpawned { get; set; }

    public override void Spawned()
    {
        if (!HasStateAuthority)
            return;

        IsSpawned = false;
        AreAllPlayersSpawned = false;
        PlayerRef = Runner.LocalPlayer;
        Managers.NetworkMng.Player = this;
        PlayerName = Managers.NetworkMng.PlayerName;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isActiveAndEnabled);

        yield return new WaitUntil(() => Object && Object.IsValid);

        yield return new WaitUntil(() => Runner && Runner.IsRunning);

        if (!Managers.SceneMng.CurrentScene.IsSceneType((int)Define.SceneType.GameScene | (int)Define.SceneType.ReadyScene | (int)Define.SceneType.TutorialScene))
            yield break;

        Creature = GetComponent<Creature>();
        if (Creature is Crew)
        {
            if (Managers.ObjectMng.MyCreature is Crew)
            {
                var ui = Managers.UIMng.MakeWorldSpaceUI<UI_NameTag>(transform);

                if (PlayerRef == Runner.LocalPlayer)
                {
                    ui.gameObject.SetActive(false);
                }
            }
            CreatureType = Define.CreatureType.Crew;
        }
        else
        {
            CreatureType = Define.CreatureType.Alien;
        }

        if (HasStateAuthority)
        {
            yield return new WaitUntil(() => Creature.IsSpawned);
            IsSpawned = true;
            yield return new WaitUntil(() => Managers.NetworkMng.AllPlayers.All((player) =>  player.IsSpawned));
            AreAllPlayersSpawned = true;
        }
    }

    private void Update()
    {
        OnPlayerNameUpdate?.Invoke((IsMaster ? " Master " : "") + PlayerName.Value);
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            IsMaster = Runner.IsSharedModeMasterClient;
            PlayerRef = Runner.LocalPlayer;
        }
    }

    public void GetReady()
    {
        if (State == Define.PlayerState.None)
        {
            State = Define.PlayerState.Ready;
        }
    }

    [Rpc]
    public static async void RPC_SpawnPlayer(NetworkRunner runner, [RpcTarget] PlayerRef player, SpawnPoint.SpawnPointData spawnPoint, bool isAlien)
    {
        NetworkObject no = isAlien
            ? await Managers.ObjectMng.SpawnAlien(Define.ALIEN_STALKER_ID, spawnPoint)
            : await Managers.ObjectMng.SpawnCrew(Define.CREW_CREWA_ID, spawnPoint, isGameScene : true);
        runner.SetPlayerObject(player, no);
    }
}
