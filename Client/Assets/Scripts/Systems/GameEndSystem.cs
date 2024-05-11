using Fusion;
using UnityEngine;

public class GameEndSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnCrewNumChanged))]
    public int CrewNum { get; set; } = Define.PLAYER_COUNT - 1;

    [Networked]
    public int KilledCrewNum { get; set; } = 0;

    public void Init()
    {
        Managers.GameMng.GameEndSystem = this;
    }

    private void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EndCrewGame(bool isWin)
    {
        ShowCursor();

        if (isWin)
        {
            Managers.UIMng.ShowPopupUI<UI_CrewWin>();
        }
        else
        {
            Managers.UIMng.ShowPopupUI<UI_CrewDefeat>();
        }

        Rpc_EndCrewGame(isWin);
    }

    private void EndAlienGame()
    {
        if (Managers.ObjectMng.MyCreature is not Alien alien)
            return;

        ShowCursor();

        if (KilledCrewNum >= Define.PLAYER_COUNT - 1)
        {
            Managers.UIMng.ShowPopupUI<UI_AlienWin>();
        }
        else
        {
            Managers.UIMng.ShowPopupUI<UI_AlienDefeat>();
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

    public async void ExitGame()
    {
        await Runner.Shutdown();
        Managers.SceneMng.LoadScene(Define.SceneType.LobbyScene);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_EndCrewGame(NetworkBool isWin)
    {
        if (!isWin)
            KilledCrewNum++;

        CrewNum--;
    }
}
