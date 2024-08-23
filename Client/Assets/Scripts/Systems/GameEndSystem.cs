using Fusion;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class GameEndSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnCrewNumChanged))]
    public int CrewNum { get; set; }
    [Networked]
    public int KilledCrewNum { get; set; } = 0;
    [Networked]
    public int DroppedCrewNum { get; set; } = 0;
    [Networked]
    public bool IsGameStarted { get; set; } = false;
    [Networked]
    public bool IsCrewKilled { get; set; } = false;
    [Networked]
    public bool IsCrewDropped { get; set; } = false;
    [Networked]
    public bool IsCrewWinning { get; set; } = false;
    [Networked]
    public int LoadedPlayerNum { get; set; } = 0;
    public float ElapsedTime { get; set; } = 0f;
    [Networked]
    public bool AreAllPlayersLoaded { get; set; } = false;

    public void Init()
    {
        Managers.GameMng.GameEndSystem = this;
        CrewNum = Define.PLAYER_COUNT - 1;

        if (Managers.NetworkMng.IsTestScene || Managers.NetworkMng.IsTutorialScene || Managers.NetworkMng.IsEndGameTriggered)
            return;

        if (Managers.NetworkMng.IsMaster)
            StartCoroutine(WaitLoadingAlarm());

        StartCoroutine(SendLoadingAlarm());
    }

    private IEnumerator WaitLoadingAlarm()
    {
        while (LoadedPlayerNum < Define.PLAYER_COUNT)
        {
            ElapsedTime += Time.deltaTime;

            if (ElapsedTime > Define.GAME_WAIT_TIME || Managers.NetworkMng.SpawnCount < Define.PLAYER_COUNT)
            {
                AreAllPlayersLoaded = true;
                RPC_EndGame(Managers.NetworkMng.Runner);
                yield break;
            }

            yield return null;
        }

        AreAllPlayersLoaded = true;
    }

    private IEnumerator SendLoadingAlarm()
    {
        while (Managers.NetworkMng.IsGameLoading == true)
        {
            yield return new WaitForSeconds(0.2f);
        }

        RPC_LoadingAlarm(Managers.NetworkMng.Runner);
    }

    public void LoadingAlarm()
    {
        LoadedPlayerNum++;
    }

    [Rpc]
    public static void RPC_LoadingAlarm(NetworkRunner runner)
    {
        if (!Managers.NetworkMng.IsMaster)
            return;

        Managers.NetworkMng.EndSystemQueue.Enqueue((system) => system.LoadingAlarm());
    }

    public void EndGame()
    {
        Managers.NetworkMng.OnAlienDropped();
    }

    [Rpc]
    public static void RPC_EndGame(NetworkRunner runner)
    {
        Managers.NetworkMng.EndSystemQueue.Enqueue((system) => system.EndGame());
    }


    public void EndCrewGame(bool isWin)
    {
        Util.ShowCursor();

        Rpc_EndCrewGame(isWin, Runner.LocalPlayer);
    }

    public async void EndAlienGame()
    {
        if (Managers.ObjectMng.MyCreature is not Alien alien)
            return;

        if (!Managers.NetworkMng.IsEndGameTriggered)
        {
            if (KilledCrewNum + DroppedCrewNum >= Define.PLAYER_COUNT - 1)
            {
                Managers.GameMng.GameResult = Define.GameResultType.AlienWin;
            }
            else
            {
                Managers.GameMng.GameResult = Define.GameResultType.AlienDefeat;
            }

            Managers.NetworkMng.IsEndGameTriggered = true;
        }

        while (Managers.NetworkMng.SpawnCount != 1)
        {
            await Task.Delay(500);
        }

        alien.OnGameEnd();
        Util.ShowCursor();
    }

    public void OnCrewNumChanged()
    {
        Managers.UIMng.CheckDroppedPlayer();

        if (CrewNum <= 0)
        {
            EndAlienGame();
        }

        if (CrewNum == 1)
        {
            Managers.GameMng.PlanSystem.EnablePlanC();
        }
    }

    public void OnCrewDropped(PlayerRef playerRef)
    {
        if (Managers.NetworkMng.IsTutorialScene || Managers.NetworkMng.IsTestScene)
            return;

        IsCrewDropped = true;
        var pd = Managers.NetworkMng.GetPlayerData(playerRef);
        if (pd.State == Define.CrewState.Alive)
        {
            CrewNum--;
            DroppedCrewNum++;
            pd.State = Define.CrewState.Disconnected;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_EndCrewGame(NetworkBool isWin, PlayerRef playerRef = default)
    {
        var pd = Managers.NetworkMng.GetPlayerData(playerRef);

        if (isWin)
        {
            IsCrewWinning = true;
            pd.State = Define.CrewState.GameWin;
        }
        else
        {
            KilledCrewNum++;
            IsCrewKilled = true;
            pd.State = Define.CrewState.GameDefeat;
        }

        CrewNum--;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ResetDropCrew()
    {
        if (Managers.NetworkMng.IsTutorialScene || Managers.NetworkMng.IsTestScene)
            return;

        IsCrewDropped = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ResetKilledCrew()
    {
        if (Managers.NetworkMng.IsTutorialScene || Managers.NetworkMng.IsTestScene)
            return;

        IsCrewKilled = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ResetWinedCrew()
    {
        if (Managers.NetworkMng.IsTutorialScene || Managers.NetworkMng.IsTestScene)
            return;

        IsCrewWinning = false;
    }
}
