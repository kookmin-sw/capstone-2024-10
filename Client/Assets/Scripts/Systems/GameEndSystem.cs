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

        if (Managers.NetworkMng.IsTestScene)
        {
            Managers.StartMng.IsGameStarted = true;
            return;
        }

        if (Managers.NetworkMng.SpawnCount != Define.PLAYER_COUNT)
        {
            if (Managers.ObjectMng.MyCreature is Alien)
            {
                StartCoroutine(EndAlienGame());
            }
            else
            {
                EndCrewGame(true);
            }
        }
    }

    public void EndCrewGame(bool isWin)
    {
        ShowCursor();

        if (isWin && !Managers.NetworkMng.IsTriggered)
        {
            Managers.UIMng.ShowPopupUI<UI_CrewWin>();
        }
        else
        {
            Managers.UIMng.ShowPopupUI<UI_CrewDefeat>();
        }

        Rpc_EndCrewGame(isWin, Runner.LocalPlayer);
    }

    public IEnumerator EndAlienGame()
    {
        if (Managers.ObjectMng.MyCreature is not Alien alien)
            yield break;

        ShowCursor();

        if (!Managers.NetworkMng.IsTriggered)
        {
            if (KilledCrewNum + DroppedCrewNum >= Define.PLAYER_COUNT - 1)
            {
                Managers.UIMng.ShowPopupUI<UI_AlienWin>();
                
            }
            else
            {
                Managers.UIMng.ShowPopupUI<UI_AlienDefeat>();

            }
        }

        yield return new WaitUntil(() => alien);

        alien.StartCoroutine("OnGameEnd");
    }

    public override void FixedUpdateNetwork()
    {
        //Debug.Log($"Killed {KilledCrewNum}, Dropped {DroppedCrewNum}, Total {CrewNum}, Crew {Managers.NetworkMng.CrewPlayerCount}, Alien {Managers.NetworkMng.AlienPlayerCount}");
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
        if (Managers.NetworkMng.IsTestScene)
            return;

        DroppedCrewNum++;
        DroppedCrew = true;
        if (Managers.NetworkMng.GetPlayerData(playerRef).State == Define.CrewState.Alive)
        {
            CrewNum--;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_EndCrewGame(NetworkBool isWin, PlayerRef playerRef = default)
    {
        if (!isWin)
        {
            KilledCrewNum++;
            KilledCrew = true;
            Managers.NetworkMng.GetPlayerData(playerRef).State = Define.CrewState.Dead;
        }

        if (isWin)
        {
            WinedCrew = true;
        }

        CrewNum--;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void RPC_ResetDropCrew()
    {
        if (Managers.NetworkMng.IsTestScene)
            return;

        DroppedCrew = false;
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    public void RPC_ResetKilledCrew()
    {
        if (Managers.NetworkMng.IsTestScene)
            return;

        KilledCrew = false;
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
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
