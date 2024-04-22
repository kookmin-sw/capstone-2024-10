using System.Collections;
using UnityEngine;

public class ReadyScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.ReadyScene;

        Managers.SoundMng.Stop(Define.SoundType.Bgm);
        Managers.UIMng.ShowPopupUI<UI_StartGame>(parent: Managers.UIMng.Root.transform);

        StartCoroutine(Managers.StartMng.TryStartGame());
    }

    public override IEnumerator OnPlayerSpawn()
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

    public override void Clear()
    {
        StopAllCoroutines();
        Managers.SoundMng.Clear();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
