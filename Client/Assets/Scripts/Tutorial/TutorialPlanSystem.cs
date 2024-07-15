using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

//이 스크립트가 튜토리얼 종료까지 총괄
public class TutorialPlanSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnBatteryCharge))]
    public int BatteryChargeCount { get; set; }
    public bool IsBatteryChargeFinished { get; private set; }

    [Networked, OnChangedRender(nameof(OnComputerWorkFinished))]
    public NetworkBool IsComputerWorkFinished { get; set; }

    [Networked, OnChangedRender(nameof(OnCardkeyUsed))]
    public NetworkBool IsCardkeyUsed { get; set; }

    private void Start()
    {
        Managers.TutorialMng.TutorialPlanSystem = this;

        if (Managers.ObjectMng.MyCreature is Crew) GameObject.FindGameObjectsWithTag("BatteryCharger").SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));
    }

    private void OnBatteryCharge()
    {
        if (Managers.ObjectMng.MyCreature is Alien) return;

        GameObject.FindWithTag("Player").GetComponent<TutorialCrew>()
            .CrewTutorialUI.TutorialPlanUI.GetComponent<UI_TutorialPlan>().UpdateBatteryCount(BatteryChargeCount);

        //TODO: DEFINE에 튜토리얼용 값 따로 추가
        if (BatteryChargeCount == 2)
        {
            IsBatteryChargeFinished = true;
        }
    }

    private void OnComputerWorkFinished()
    {
        if (Managers.ObjectMng.MyCreature is Alien) return;

        GameObject.Find("CargoPassageGate").GetComponent<Gate>().Open();
    }

    private void OnCardkeyUsed()
    {
        if (Managers.ObjectMng.MyCreature is Alien) return;

        var ui = Managers.ObjectMng.MyCrew.CrewIngameUI as UI_CrewTutorial;
        ui.TutorialPlanUI.GetComponent<UI_TutorialPlan>().OnCardkeyUsed();
    }

    public void EndTutorial()
    {
        Managers.SceneMng.LoadScene(0);
    }
}
