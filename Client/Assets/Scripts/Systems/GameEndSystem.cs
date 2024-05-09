using Fusion;
using UnityEngine;

public class GameEndSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(AlienEndGame))]
    public int CrewNum { get; set; } = Define.PLAYER_COUNT - 1;

    [Networked]
    public int KilledCrewNum { get; set; } = 0;

    public void Init()
    {
        Managers.GameMng.GameEndSystem = this;
    }

    public void CreatureEndGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CrewEndGame(bool isWin)
    {
        CreatureEndGame();
        if (isWin)
        {
            Managers.UIMng.ShowPopupUI<UI_CrewWin>();
        }
        else
        {
            Managers.UIMng.ShowPopupUI<UI_CrewDefeat>();
        }

        Rpc_CrewEndGame(isWin);
    }

    public void AlienEndGame()
    {
        if (Managers.ObjectMng.MyCreature is not Alien alien)
            return;

        if (CrewNum <= 0)
        {
            CreatureEndGame();
            if (KilledCrewNum >= Define.PLAYER_COUNT - 1)
            {
                Managers.UIMng.ShowPopupUI<UI_AlienWin>();
                alien.OnEndGame();
            }
            else
            {
                Managers.UIMng.ShowPopupUI<UI_AlienDefeat>();
                alien.OnEndGame();
                //alien.Rpc_OnEndGame();
            }
        }
    }

    public async void Exit()
    {
        await Runner.Shutdown();
        Managers.SceneMng.LoadScene(Define.SceneType.LobbyScene);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_CrewEndGame(NetworkBool isWin)
    {
        if (!isWin)
            KilledCrewNum++;

        CrewNum--;
    }
}
