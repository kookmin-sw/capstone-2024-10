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
                Managers.UIMng.ShowPanelUI<UI_CrewWin>(Managers.UIMng.Root.transform);
                PlayEndSound();
                break;
            case Define.GameResultType.CrewDefeat:
                Managers.UIMng.ShowPanelUI<UI_CrewDefeat>(Managers.UIMng.Root.transform);
                break;
            case Define.GameResultType.AlienWin:
                Managers.UIMng.ShowPanelUI<UI_AlienWin>(Managers.UIMng.Root.transform);
                PlayEndSound();
                break;
            case Define.GameResultType.AlienDefeat:
                Managers.UIMng.ShowPanelUI<UI_AlienDefeat>(Managers.UIMng.Root.transform);
                PlayEndSound();
                break;
        }
    }

    private void OnApplicationQuit()
    {
        Clear();
    }

    public override void Clear()
    {
        StopAllCoroutines();
    }

    public void PlayEndSound()
    {
        Managers.SoundMng.Play($"{Define.BGM_PATH}/Panic Man", Define.SoundType.Bgm, volume: 0.6f);
    }
}
