using System.Collections;
using UnityEngine;

public class ReadyScene : BaseScene
{
    // 씬이 초기에 생성될 때 수행될 목록
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.ReadyScene;

        Managers.UIMng.ShowPopupUI<UI_StartGame>(parent: Managers.UIMng.Root.transform);
        StartCoroutine(Managers.GameMng.TryStartGame());
    }

    public override IEnumerator OnSceneLoaded()
    {
        UI_CrewIngame ingameUI = Managers.UIMng.ShowSceneUI<UI_CrewIngame>();
        yield return new WaitUntil(() => ingameUI.Init());

        ingameUI.InitAfterNetworkSpawn(Managers.ObjectMng.MyCreature);
        Managers.ObjectMng.MyCreature.IngameUI = ingameUI;
        ingameUI.UI_CrewHP.gameObject.SetActive(false);
        ingameUI.UI_CrewStamina.gameObject.SetActive(false);
        ingameUI.ObjectiveUI.gameObject.SetActive(false);
        ingameUI.UI_Inventory.gameObject.SetActive(false);
    }

    // 씬이 바뀔 때 정리해야 하는 목록
    public override void Clear()
    {
        Managers.SoundMng.Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
