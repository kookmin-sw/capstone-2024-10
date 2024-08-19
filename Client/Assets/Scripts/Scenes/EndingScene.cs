using System.Collections;
using UnityEngine;

public class EndingScene : BaseScene
{
    private Define.GameResultType _gameResult = Define.GameResultType.NotDecided;
    protected override async void Init()
    {
        base.Init();

        SceneType = Define.SceneType.EndingScene;
        _gameResult = Managers.GameMng.GameResult;

        await Managers.NetworkMng.Runner.Shutdown();
        Debug.Log(_gameResult);
        switch (_gameResult)
        {
            case Define.GameResultType.CrewWin:
                break;
            case Define.GameResultType.CrewDefeat:
                Managers.UIMng.ShowPanelUI<UI_CrewDefeat>(parent: Managers.UIMng.Root.transform);
                break;
            case Define.GameResultType.AlienWin:
                break;
            case Define.GameResultType.AlienDefeat:
                break;
        }
    }
    
    public override void Clear()
    {
        StopAllCoroutines();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }

}
