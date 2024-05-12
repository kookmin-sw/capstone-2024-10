using System.Collections;
using UnityEngine;

public class ReadyScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.ReadyScene;

        Managers.SoundMng.Play($"{Define.BGM_PATH}/Tone Hum", Define.SoundType.Bgm, volume:1f);

        Managers.UIMng.ShowPopupUI<UI_StartGame>(parent: Managers.UIMng.Root.transform);

        StartCoroutine(Managers.StartMng.TryStartGame());

        SettingSystem settingSystem = FindAnyObjectByType<SettingSystem>();
        settingSystem.Init();
    }

    public override IEnumerator OnPlayerSpawn()
    {
        UI_CrewIngame ingameUI = Managers.UIMng.ShowSceneUI<UI_CrewIngame>();
        yield return new WaitUntil(() => ingameUI.Init());

        ingameUI.InitAfterNetworkSpawn(Managers.ObjectMng.MyCreature);
        Managers.ObjectMng.MyCreature.IngameUI = ingameUI;
        ingameUI.CrewHpUI.gameObject.SetActive(false);
        ingameUI.CrewStaminaUI.gameObject.SetActive(false);
        ingameUI.PlanUI.gameObject.SetActive(false);
        ingameUI.InventoryUI.gameObject.SetActive(false);
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
