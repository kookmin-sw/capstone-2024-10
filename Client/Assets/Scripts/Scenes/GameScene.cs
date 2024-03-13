public class GameScene : BaseScene
{
    // 씬이 초기에 생성될 때 수행될 목록
    protected override void Init()
    {
        base.Init();
        SceneType = Define.SceneType.GameScene;

        //Managers.ResourceMng.Instantiate("Camera");
        Managers.MapMng.Init();
        //UI 세팅
        // Managers.UI.ShowSceneUI<UI_MainScene>();
    }

    // 씬이 바뀔 때 정리해야 하는 목록
    public override void Clear()
    {
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
