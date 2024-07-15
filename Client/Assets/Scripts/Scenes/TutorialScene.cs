using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.TutorialScene;

        Managers.SoundMng.Stop(Define.SoundType.Bgm);
        Managers.SoundMng.Play($"{Define.BGM_PATH}/Tone Hum", Define.SoundType.Environment, volume:1f);

        SettingSystem settingSystem = FindObjectOfType<SettingSystem>();
        settingSystem.Init();
    }

    public override IEnumerator OnPlayerSpawn()
    {
        MapSystem mapSystem = null;
        PlanSystem planSystem = null;
        GameEndSystem gameEndSystem = null;
        while (mapSystem == null || planSystem == null)
        {
            mapSystem = FindObjectOfType<MapSystem>();
            planSystem = FindObjectOfType<PlanSystem>();
            gameEndSystem = FindObjectOfType<GameEndSystem>();

            yield return new WaitForSeconds(0.5f);
        }
        mapSystem.Init();
        planSystem.Init();
        gameEndSystem.Init();
        UI_Ingame ingameUI = Managers.UIMng.ShowSceneUI<UI_CrewTutorial>();

        yield return new WaitUntil(() => ingameUI.Init());

        ingameUI.InitAfterNetworkSpawn(Managers.ObjectMng.MyCreature);
        Managers.ObjectMng.MyCreature.IngameUI = ingameUI;

        gameEndSystem.InitAfterUIPopup();

        var loadingUI = Managers.UIMng.PanelUI as UI_Loading;
        // 테스트 씬은 로딩 UI를 띄우지 않음
        if (loadingUI != null)
        {
            loadingUI.OnMapLoadComplete();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (FindAnyObjectByType<UI_ExitGame>() == null &&
                FindAnyObjectByType<UI_SettingPanel>() == null &&
                FindAnyObjectByType<UI_ManualPanel>() == null)
                Managers.UIMng.ShowPanelUI<UI_ExitGame>();
        }
    }

    public override void Clear()
    {
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
