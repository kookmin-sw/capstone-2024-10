using System.Collections;
using UnityEngine;

public class ReadyScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.ReadyScene;

        Managers.SoundMng.Stop(Define.SoundType.Bgm);
        Managers.SoundMng.Play($"{Define.BGM_PATH}/Tone Hum", Define.SoundType.Environment, volume:1f);

        Managers.UIMng.ShowPopupUI<UI_StartGame>();

        StartCoroutine(Managers.StartMng.TryStartGame());

        SettingSystem settingSystem = FindAnyObjectByType<SettingSystem>();
        settingSystem.Init();
    }

    public override IEnumerator OnPlayerSpawn()
    {
        UI_CrewIngame ingameUI = Managers.UIMng.ShowSceneUI<UI_CrewIngame>();
        yield return new WaitUntil(() => ingameUI.Init());

        Managers.ObjectMng.MyCreature.IngameUI = ingameUI;
        ingameUI.InitAfterNetworkSpawn(Managers.ObjectMng.MyCreature);
        ingameUI.CrewHpUI.gameObject.SetActive(false);
        ingameUI.CrewStaminaUI.gameObject.SetActive(false);
        ingameUI.PlanUI.gameObject.SetActive(false);
        ingameUI.InventoryUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (FindAnyObjectByType<UI_ExitGame>() == null &&
                FindAnyObjectByType<UI_SettingPanel>() == null &&
                FindAnyObjectByType<UI_GuidePanel>() == null)
                Managers.UIMng.ShowPanelUI<UI_ExitGame>();
        }
    }

    public override void Clear()
    {
        Managers.SoundMng.Clear();
    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
        Clear();
    }
}
