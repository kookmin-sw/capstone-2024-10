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
    public bool ShouldEndGame { get; set; } = false;
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

    public void Init()
    {
        Managers.GameMng.GameEndSystem = this;
        CrewNum = Define.PLAYER_COUNT - 1;
    }

    public void InitAfterUIPopup()
    {
        if (Managers.NetworkMng.IsTestScene || Managers.NetworkMng.IsEndGameTriggered)
            return;

        if (Managers.NetworkMng.SpawnCount != Define.PLAYER_COUNT)
        {
            if (ShouldEndGame == false)
                RPC_EndGameRequest(Managers.NetworkMng.Runner);
        }
    }

    public void EndGameRequest()
    {
        if (ShouldEndGame)
            return;

        ShouldEndGame = true;
        RPC_EndGame(Managers.NetworkMng.Runner);
    }

    [Rpc]
    public static void RPC_EndGameRequest(NetworkRunner runner)
    {
        if (!Managers.NetworkMng.IsMaster)
            return;

        Managers.NetworkMng.EndSystemQueue.Enqueue((system) => system.EndGameRequest());
    }

    public void EndGame()
    {
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
