using System.Collections;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.GameScene;

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
        UI_Ingame ingameUI = Managers.ObjectMng.MyCreature is Crew ? Managers.UIMng.ShowSceneUI<UI_CrewIngame>() : Managers.UIMng.ShowSceneUI<UI_AlienIngame>();
        yield return new WaitUntil(() => ingameUI.Init());

        ingameUI.InitAfterNetworkSpawn(Managers.ObjectMng.MyCreature);
        Managers.ObjectMng.MyCreature.IngameUI = ingameUI;

        var loadingUI = Managers.UIMng.PanelUI as UI_Loading;
        Debug.Assert(loadingUI != null);
        loadingUI.OnMapLoadComplete();
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

    // 씬이 바뀔 때 정리해야 하는 목록
    public override void Clear()
    {
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
