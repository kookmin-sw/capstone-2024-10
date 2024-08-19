using UnityEngine;
using Fusion;

public class PlanSystem : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnBatteryCharge))]
    public int BatteryChargeCount { get; set; }
    [Networked, OnChangedRender(nameof(OnUSBKeyInsert))]
    public int USBKeyInsertCount { get; set; }
    public bool IsBatteryChargeFinished { get; private set; }
    public bool IsUSBKeyInsertFinished { get; private set; }

    [Networked, OnChangedRender(nameof(OnCardkeyUsed))]
    public NetworkBool IsCardKeyUsed { get; set; }
    [Networked, OnChangedRender(nameof(OnCentralComputerWorkFinished))]
    public NetworkBool IsCentralComputerWorkFinished { get; set; }
    [Networked, OnChangedRender(nameof(OnCargoGateComputerUsed))]
    public NetworkBool IsCargoPassageOpen { get; set; }
    [Networked, OnChangedRender(nameof(OnPanicRoomActivated))]
    public NetworkBool IsPanicRoomActivated { get; set; }

    public void Init()
    {
        Managers.GameMng.PlanSystem = this;
        GameObject.FindGameObjectsWithTag("BatteryCharger").SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));
    }

    private void OnBatteryCharge()
    {
        if (BatteryChargeCount == Define.BATTERY_CHARGE_GOAL)
        {
            Managers.SoundMng.Play($"{Define.FACILITY_PATH}/Plan_BatteryCharge", type: Define.SoundType.Facility, volume:0.4f, isOneShot:true);
            IsBatteryChargeFinished = true;
            GameObject.FindGameObjectsWithTag("BatteryCharger").SetLayerRecursive(LayerMask.NameToLayer("MapObject"));
            GameObject.FindGameObjectsWithTag("CentralControlComputer").SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));
            GameObject.FindGameObjectsWithTag("ElevatorControlComputer").SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));
        }

        if (Managers.ObjectMng.MyCreature is Crew crew && crew.CrewIngameUI && crew.CrewIngameUI.PlanUI)
            crew.CrewIngameUI.PlanUI.UpdateBatteryCount(BatteryChargeCount);
    }

    private void OnUSBKeyInsert()
    {
        if (USBKeyInsertCount == Define.USBKEY_INSERT_GOAL)
        {
            Managers.SoundMng.Play($"{Define.FACILITY_PATH}/Plan_A", type: Define.SoundType.Facility, volume:0.7f, isOneShot:true);
            IsUSBKeyInsertFinished = true;
            GameObject.FindGameObjectsWithTag("ElevatorControlComputer").SetLayerRecursive(LayerMask.NameToLayer("MapObject"));

            if (Managers.ObjectMng.MyCreature is Crew) GameObject.FindGameObjectsWithTag("ElevatorKeypad").SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));
            else GameObject.FindGameObjectsWithTag("ElevatorDoor").SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));
        }

        if (Managers.ObjectMng.MyCreature is Crew crew && crew.CrewIngameUI && crew.CrewIngameUI.PlanUI)
            crew.CrewIngameUI.PlanUI.UpdateUSBKeyCount(USBKeyInsertCount);
    }

    private void OnCardkeyUsed()
    {
        if (Managers.ObjectMng.MyCreature is Crew crew && crew.CrewIngameUI && crew.CrewIngameUI.PlanUI)
            crew.CrewIngameUI.PlanUI.OnCardkeyUsed();
    }

    private void OnCentralComputerWorkFinished()
    {
        GameObject.FindGameObjectsWithTag("CentralControlComputer").SetLayerRecursive(LayerMask.NameToLayer("MapObject"));
        GameObject.FindGameObjectsWithTag("CargoPassageControlComputer").SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));

        if (Managers.ObjectMng.MyCreature is Crew crew && crew.CrewIngameUI && crew.CrewIngameUI.PlanUI)
            crew.CrewIngameUI.PlanUI.OnCentralControlComputerWorkFinished();
    }

    private void OnCargoGateComputerUsed()
    {
        Managers.SoundMng.Play($"{Define.FACILITY_PATH}/Plan_B", type: Define.SoundType.Facility, volume: 0.9f, isOneShot:true);

        GameObject.FindGameObjectsWithTag("CargoPassageControlComputer").SetLayerRecursive(LayerMask.NameToLayer("MapObject"));
        GameObject.FindGameObjectsWithTag("CargoPassageGate").SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));

        if (Managers.ObjectMng.MyCreature is Crew crew && crew.CrewIngameUI && crew.CrewIngameUI.PlanUI)
            crew.CrewIngameUI.PlanUI.OnCargoPassageOpen();
    }

    public void EnablePlanC()
    {
        if (!IsBatteryChargeFinished) return;

        GameObject.FindGameObjectsWithTag("EmergencyControlDevice").SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));

        if (Managers.ObjectMng.MyCreature is Crew crew && crew.CrewIngameUI && crew.CrewIngameUI.PlanUI)
            crew.CrewIngameUI.PlanUI.EnablePlanC();
    }

    private void OnPanicRoomActivated()
    {
        GameObject.FindGameObjectsWithTag("EmergencyControlDevice").SetLayerRecursive(LayerMask.NameToLayer("MapObject"));

        if (Managers.ObjectMng.MyCreature is Crew crew && crew.CrewIngameUI && crew.CrewIngameUI.PlanUI)
        {
            crew.CrewIngameUI.PlanUI.OnPanicRoomActivated();
            GameObject.FindGameObjectsWithTag("PanicRoom").SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));
        }
        //else
        //{
        //    GameObject.FindGameObjectsWithTag("PanicRoomDoor").SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));
        //}
    }
}
