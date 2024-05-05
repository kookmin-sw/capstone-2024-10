using System;
using System.Collections;
using UnityEngine;
using Fusion;

public class PlanSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnBatteryCharge))]
    public int BatteryChargeCount { get; set; }

    public bool IsBatteryChargeFinished { get; private set; }
    public bool IsKeyCardUsed { get; private set; }

    [Networked, OnChangedRender(nameof(OnCardkeyUsed))]
    public NetworkBool IsCardkeyUsed { get; set; }

    public void Init()
    {
        Managers.GameMng.PlanSystem = this;
        Managers.ObjectMng.MyCrew.CrewIngameUI.PlanUI.UpdateBatteryCount(0);
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

    private void OnCardkeyUsed()
    {
        if (Managers.ObjectMng.MyCreature is Alien) return;

        Managers.ObjectMng.MyCrew.CrewIngameUI.PlanUI.OnKeycardUsed();
    }
}
