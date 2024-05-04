using System;
using System.Collections;
using UnityEngine;
using Fusion;

public class PlanSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnBatteryCharge))] public int BatteryChargeCount { get; set; }
    public bool IsBatteryChargeFinished { get; private set; }
    [Networked, OnChangedRender(nameof(OnGeneratorRestored))] public NetworkBool IsGeneratorRestored { get; set; }

    public void Init()
    {
        Managers.GameMng.PlanSystem = this;
    }
    
    private void OnBatteryCharge()
    {
        if (Managers.ObjectMng.MyCreature is Alien) return;

        Managers.ObjectMng.MyCrew.CrewIngameUI.PlanUI.UpdateBatteryCount(BatteryChargeCount);

        if (BatteryChargeCount == Define.BATTERY_COLLECT_GOAL)
        {
            IsBatteryChargeFinished = true;
        }
    }

    private void OnGeneratorRestored()
    {
        if (Managers.ObjectMng.MyCreature is Alien) return;

        Managers.ObjectMng.MyCrew.CrewIngameUI.PlanUI.OnGeneratorRestored();
    }
}
