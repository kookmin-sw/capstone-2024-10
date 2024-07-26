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
    public NetworkBool ShouldEndGame { get; set; } = false;
    [Networked]
    public int DroppedCrewNum { get; set; } = 0;
    [Networked]
    public NetworkBool IsGameStarted { get; set; } = false;
    [Networked]
    public NetworkBool IsCrewKilled { get; set; } = false;
    [Networked]
    public NetworkBool IsCrewDropped { get; set; } = false;
    [Networked]
    public NetworkBool IsCrewWinning { get; set; } = false;
    [Networked]
    public int LoadedPlayerNum { get; set; } = 0;
    public float ElapsedTime { get; set; } = 0f;
    [Networked]
    public NetworkBool LoadingDone { get; set; } = false;

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

            if (ElapsedTime > Define.GAME_WAIT_TIME)
            {
                RPC_EndGame(Managers.NetworkMng.Runner);
                yield break;
            }

            yield return null;
        }

        LoadingDone = true;
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
        Managers.UIMng.BlockLoadingUI(false);
        if (Managers.ObjectMng.MyCreature is Alien)
        {
            StartCoroutine(EndAlienGame());
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

        if (!Managers.NetworkMng.IsEndGameTriggered)
        {
            if (isWin)
            {
                Managers.UIMng.ShowPopupUI<UI_CrewWin>();
            }
            else
            {
                Managers.UIMng.ShowPopupUI<UI_CrewDefeat>();
            }

            Managers.NetworkMng.IsEndGameTriggered = true;
        }

        Rpc_EndCrewGame(isWin, Runner.LocalPlayer);
    }

    public IEnumerator EndAlienGame()
    {
        if (Managers.ObjectMng.MyCreature is not Alien alien)
            yield break;

        ShowCursor();

        if (!Managers.NetworkMng.IsEndGameTriggered)
        {
            if (KilledCrewNum + DroppedCrewNum >= Define.PLAYER_COUNT - 1)
            {
                Managers.UIMng.ShowPopupUI<UI_AlienWin>();
            }
            else
            {
                Managers.UIMng.ShowPopupUI<UI_AlienDefeat>();
            }

            Managers.NetworkMng.IsEndGameTriggered = true;
        }

        yield return new WaitUntil(() => alien);

        StartCoroutine(alien.OnGameEnd());
    }

    public void OnCrewNumChanged()
    {
        if (CrewNum <= 0)
        {
            StartCoroutine(EndAlienGame());
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
