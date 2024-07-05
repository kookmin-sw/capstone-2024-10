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
    public int DroppedAlienNum { get; set; } = 0;
    [Networked]
    public bool IsGameStarted { get; set; } = false;
    [Networked]
    public bool KilledCrew { get; set; } = false;
    [Networked]
    public bool DroppedCrew { get; set; } = false;
    [Networked]
    public bool WinedCrew { get; set; } = false;

    public void Init()
    {
        Managers.GameMng.GameEndSystem = this;
        CrewNum = Define.PLAYER_COUNT - 1;
    }

    public async void InitAfterUIPopup()
    {
        if (Managers.NetworkMng.IsTestScene || Managers.NetworkMng.IsEndGameTriggered)
            return;

        if (Managers.NetworkMng.SpawnCount == Define.PLAYER_COUNT)
            return;

        // 마지막 크루가 로드되었을 때를 기준으로
        if (ShouldEndGame == false)
            RPC_EndGameRequest();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_EndGameRequest()
    {
        ShouldEndGame = true;
        RPC_EndGame();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_EndGame()
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

        DroppedCrew = true;
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
            WinedCrew = true;
        }
        else
        {
            KilledCrewNum++;
            KilledCrew = true;
        }

        Managers.NetworkMng.GetPlayerData(playerRef).State = Define.CrewState.GameEnd;
        CrewNum--;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ResetDropCrew()
    {
        if (Managers.NetworkMng.IsTestScene)
            return;

        DroppedCrew = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ResetKilledCrew()
    {
        if (Managers.NetworkMng.IsTestScene)
            return;

        KilledCrew = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ResetWinedCrew()
    {
        if (Managers.NetworkMng.IsTestScene)
            return;

        WinedCrew = false;
    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
