using System.Collections;
using UnityEngine;

public class GameScene : BaseScene
{
    // 씬이 초기에 생성될 때 수행될 목록
    protected override void Init()
    {
        base.Init();
        SceneType = Define.SceneType.GameScene;
    }

    public override IEnumerator OnSceneLoaded()
    {
        MapSystem mapSystem = null;
        PlanSystem planSystem = null;
        while (mapSystem == null || planSystem == null)
        {
            mapSystem = FindObjectOfType<MapSystem>();
            planSystem = FindObjectOfType<PlanSystem>();
            yield return new WaitForSeconds(0.5f);
        }
        mapSystem.Init();
        planSystem.Init();


        UI_Ingame ingameUI = Managers.ObjectMng.MyCreature is Crew ? Managers.UIMng.ShowSceneUI<UI_CrewIngame>() : Managers.UIMng.ShowSceneUI<UI_AlienIngame>();
        yield return new WaitUntil(() => ingameUI.Init());

        ingameUI.InitAfterNetworkSpawn(Managers.ObjectMng.MyCreature);
        Managers.ObjectMng.MyCreature.IngameUI = ingameUI;
        Managers.SoundMng.Clear();
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
