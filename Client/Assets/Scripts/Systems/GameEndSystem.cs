using Fusion;
using System.Collections;
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

        if (Managers.NetworkMng.IsTestScene || Managers.NetworkMng.IsEndGameTriggered)
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

            if (ElapsedTime > Define.GAME_WAIT_TIME && Managers.NetworkMng.SpawnCount < Define.PLAYER_COUNT)
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
        if (Managers.ObjectMng.MyCreature is Alien)
        {
            EndAlienGame();
        }
        else
        {
            Managers.ObjectMng.MyCrew.OnWin();
        }
    }

    [Rpc]
    public static void RPC_EndGame(NetworkRunner runner)
    {
        Managers.NetworkMng.EndSystemQueue.Enqueue((system) => system.EndGame());
    }


    public void EndCrewGame(bool isWin)
    {
        ShowCursor();

        //if (!Managers.NetworkMng.IsEndGameTriggered)
        //{
        //    if (isWin)
        //    {
        //        Managers.UIMng.ShowPanelUI<UI_CrewWin>(parent: Managers.UIMng.Root.transform);
        //    }
        //    else
        //    {
        //        Managers.UIMng.ShowPanelUI<UI_CrewDefeat>(parent: Managers.UIMng.Root.transform);
        //    }

        //    Managers.NetworkMng.IsEndGameTriggered = true;
        //}

        Rpc_EndCrewGame(isWin, Runner.LocalPlayer);
    }

    public void EndAlienGame()
    {
        if (Managers.ObjectMng.MyCreature is not Alien alien)
            return;

        ShowCursor();

        if (!Managers.NetworkMng.IsEndGameTriggered)
        {
            if (KilledCrewNum + DroppedCrewNum >= Define.PLAYER_COUNT - 1)
            {
                Managers.GameMng.GameResult = Define.GameResultType.AlienWin;
                //Managers.UIMng.ShowPanelUI<UI_AlienWin>(parent: Managers.UIMng.Root.transform);
            }
            else
            {
                Managers.GameMng.GameResult = Define.GameResultType.AlienDefeat;
                //Managers.UIMng.ShowPanelUI<UI_AlienDefeat>(parent: Managers.UIMng.Root.transform);
            }

            Managers.NetworkMng.IsEndGameTriggered = true;
        }

        alien.OnGameEnd();
    }

    public void OnCrewNumChanged()
    {
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
        if (Managers.NetworkMng.IsTestScene)
            return;

        IsCrewDropped = true;
        if (Managers.NetworkMng.GetPlayerData(playerRef).State == Define.CrewState.Alive)
        {
            CrewNum--;
            DroppedCrewNum++;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_EndCrewGame(NetworkBool isWin, PlayerRef playerRef = default)
    {
        if (isWin)
        {
            IsCrewWinning = true;
        }
        else
        {
            KilledCrewNum++;
            IsCrewKilled = true;
        }

        var pd = Managers.NetworkMng.GetPlayerData(playerRef);
        if (pd != null)
            pd.State = Define.CrewState.GameEnd;
        CrewNum--;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ResetDropCrew()
    {
        if (Managers.NetworkMng.IsTestScene)
            return;

        IsCrewDropped = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ResetKilledCrew()
    {
        if (Managers.NetworkMng.IsTestScene)
            return;

        IsCrewKilled = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ResetWinedCrew()
    {
        if (Managers.NetworkMng.IsTestScene)
            return;

        IsCrewWinning = false;
    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
