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
    public bool KilledCrew { get; set; } = false;
    [Networked]
    public bool DroppedCrew { get; set; } = false;
    [Networked]
    public bool WinedCrew { get; set; } = false;

    public void Init()
    {
        Managers.GameMng.GameEndSystem = this;
        CrewNum = Define.PLAYER_COUNT - 1;

        if (Managers.NetworkMng.IsEndGameTriggered)
        {
            Managers.StartMng.IsGameStarted = true;
        }
    }

    public void InitAfterUIPopup()
    {
        if (Managers.NetworkMng.SpawnCount != Define.PLAYER_COUNT && !Managers.NetworkMng.IsEndGameTriggered)
        {
            // 로딩 중간에 끊겼을 때, Crew 혹은 Alien이 스폰이 되지 않는 경우가 있다
            if (Managers.ObjectMng.MyCreature is Alien)
            {
                StartCoroutine(EndAlienGame());
            }
            else
            {
                Managers.ObjectMng.MyCrew.OnWin();
            }

            Managers.NetworkMng.IsEndingTriggered = true;
        }
    }

    public void EndCrewGame(bool isWin)
    {
        ShowCursor();

        if (!Managers.NetworkMng.IsEndingTriggered)
        {
            if (isWin)
            {
                Managers.UIMng.ShowPopupUI<UI_CrewWin>();
            }
            else
            {
                Managers.UIMng.ShowPopupUI<UI_CrewDefeat>();
            }

            Managers.NetworkMng.IsEndingTriggered = true;
        }

        Rpc_EndCrewGame(isWin, Runner.LocalPlayer);
    }

    public IEnumerator EndAlienGame()
    {
        if (Managers.ObjectMng.MyCreature is not Alien alien)
            yield break;

        ShowCursor();

        if (!Managers.NetworkMng.IsEndingTriggered)
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

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void RPC_OnCrewDropped(PlayerRef playerRef)
    {
        if (Managers.NetworkMng.IsEndGameTriggered)
            return;

        DroppedCrew = true;
        if (Managers.NetworkMng.GetPlayerData(playerRef).State == Define.CrewState.Alive)
        {
            CrewNum--;
            DroppedCrewNum++;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
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

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ResetDropCrew()
    {
        if (Managers.NetworkMng.IsEndGameTriggered)
            return;

        DroppedCrew = false;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ResetKilledCrew()
    {
        if (Managers.NetworkMng.IsEndGameTriggered)
            return;

        KilledCrew = false;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_ResetWinedCrew()
    {
        if (Managers.NetworkMng.IsEndGameTriggered)
            return;

        WinedCrew = false;
    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
