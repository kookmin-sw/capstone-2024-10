using System.Collections;
using UnityEngine;

public class GameScene : BaseScene
{
    // 씬이 초기에 생성될 때 수행될 목록
    protected override void Init()
    {
        base.Init();
        SceneType = Define.SceneType.GameScene;

        Managers.MapMng.Init();
    }

    public IEnumerator OnSceneLoaded()
    {
        FindObjectOfType<MapSystem>().Init();
        UI_Ingame ingameUI = Managers.ObjectMng.MyCreature is Crew ? Managers.UIMng.ShowSceneUI<UI_CrewIngame>() : Managers.UIMng.ShowSceneUI<UI_AlienIngame>();
        Debug.Log($"Onsceneloaded: {Managers.ObjectMng.MyCreature is Alien}");
        yield return new WaitUntil(() => ingameUI.Init());
        ingameUI.AssignCreature(Managers.ObjectMng.MyCreature);
        Managers.ObjectMng.MyCreature.IngameUI = ingameUI;

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
