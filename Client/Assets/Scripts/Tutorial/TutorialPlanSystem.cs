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

    public void Init()
    {
        Managers.TutorialMng.TutorialPlanSystem = this;
        if (Managers.ObjectMng.MyCreature is Crew) GameObject.FindGameObjectsWithTag("BatteryCharger").SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));
    }

    private void OnBatteryCharge()
    {
        if (Managers.ObjectMng.MyCreature is Alien) return;

        Managers.ObjectMng.MyCrew.CrewIngameUI.PlanUI.UpdateBatteryCount(BatteryChargeCount);

        if (BatteryChargeCount == Define.BATTERY_CHARGE_GOAL)
        {
            IsBatteryChargeFinished = true;
        }
    }

    private void OnComputerWorkFinished()
    {
        if (Managers.ObjectMng.MyCreature is Alien) return;


    }
}
