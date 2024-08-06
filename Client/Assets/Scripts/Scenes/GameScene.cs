using System.Collections;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();

        SceneType = Define.SceneType.GameScene;

        Managers.SoundMng.Stop(Define.SoundType.Bgm);
        Managers.SoundMng.Play($"{Define.BGM_PATH}/Space Wind 01", Define.SoundType.Environment, volume:0.15f);

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
        Managers.UIMng.OnMapLoadComplete();

        yield return new WaitUntil(() => Managers.NetworkMng.Runner.IsRunning && Managers.GameMng.GameEndSystem.AreAllPlayersLoaded);

        Managers.UIMng.BlockLoadingUI(false);
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
        StopAllCoroutines();
    }

    private void OnApplicationQuit()
    {
        Clear();
    }
}
