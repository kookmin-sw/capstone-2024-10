using System.Collections;
using UnityEngine;
using Fusion;
using System;

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
    public bool IsSpawned { get; set; } = false;

    public override void Spawned()
    {
        if (!HasStateAuthority)
            return;

        PlayerRef = Runner.LocalPlayer;
        Managers.NetworkMng.Player = this;
        PlayerName = Managers.NetworkMng.PlayerName;
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => isActiveAndEnabled);

        yield return new WaitUntil(() => Object && Object.IsValid);

        yield return new WaitUntil(() => Runner && Runner.IsRunning);

        if (!Managers.SceneMng.CurrentScene.IsSceneType((int)Define.SceneType.GameScene | (int)Define.SceneType.ReadyScene))
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

        IsSpawned = true;
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
