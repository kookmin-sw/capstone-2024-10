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
        if (Managers.ObjectMng.MyCreature is Crew) GameObject.FindGameObjectsWithTag("BatteryCharger").SetLayerRecursive(LayerMask.NameToLayer("InteractableObject"));
    }

    private void OnBatteryCharge()
    {
        if (BatteryChargeCount == Define.BATTERY_CHARGE_GOAL)
        {
            Managers.SoundMng.Play($"{Define.FACILITY_PATH}/Plan_BatteryCharge", type: Define.SoundType.Facility, volume:0.4f, isLoop: false);
        }

        if (Managers.ObjectMng.MyCreature is Alien) return;

        if (BatteryChargeCount == Define.BATTERY_CHARGE_GOAL)
        {
            IsBatteryChargeFinished = true;
            GameObject.FindGameObjectsWithTag("BatteryCharger").SetLayerRecursive(LayerMask.NameToLayer("MapObject"));
            GameObject.FindGameObjectsWithTag("CentralControlComputer").SetLayerRecursive(LayerMask.NameToLayer("InteractableObject"));
            GameObject.FindGameObjectsWithTag("ElevatorControlComputer").SetLayerRecursive(LayerMask.NameToLayer("InteractableObject"));
        }

        Managers.ObjectMng.MyCrew.CrewIngameUI.PlanUI.UpdateBatteryCount(BatteryChargeCount);
    }

    private void OnUSBKeyInsert()
    {
        if (USBKeyInsertCount == Define.USBKEY_INSERT_GOAL)
        {
            Managers.SoundMng.Play($"{Define.FACILITY_PATH}/Plan_A", type: Define.SoundType.Facility, volume:0.7f, isLoop: false);
        }

        if (Managers.ObjectMng.MyCreature is Alien) return;

        Managers.ObjectMng.MyCrew.CrewIngameUI.PlanUI.UpdateUSBKeyCount(USBKeyInsertCount);

        if (USBKeyInsertCount == Define.USBKEY_INSERT_GOAL)
        {
            IsUSBKeyInsertFinished = true;
            GameObject.FindGameObjectsWithTag("ElevatorControlComputer").SetLayerRecursive(LayerMask.NameToLayer("MapObject"));
            GameObject.FindGameObjectsWithTag("ElevatorKeypad").SetLayerRecursive(LayerMask.NameToLayer("InteractableObject"));
        }
    }

    private void OnCardkeyUsed()
    {
        if (Managers.ObjectMng.MyCreature is Alien) return;

        Managers.ObjectMng.MyCrew.CrewIngameUI.PlanUI.OnCardkeyUsed();
    }

    private void OnCentralComputerWorkFinished()
    {
        if (Managers.ObjectMng.MyCreature is Alien) return;

        Managers.ObjectMng.MyCrew.CrewIngameUI.PlanUI.OnCentralControlComputerWorkFinished();
        GameObject.FindGameObjectsWithTag("CentralControlComputer").SetLayerRecursive(LayerMask.NameToLayer("MapObject"));
        GameObject.FindGameObjectsWithTag("CargoPassageControlComputer").SetLayerRecursive(LayerMask.NameToLayer("InteractableObject"));
    }

    private void OnCargoGateComputerUsed()
    {
        Managers.SoundMng.Play($"{Define.FACILITY_PATH}/Plan_B", type: Define.SoundType.Facility, volume: 0.9f, isOneShot:true);

        if (Managers.ObjectMng.MyCreature is Alien) return;

        Managers.ObjectMng.MyCrew.CrewIngameUI.PlanUI.OnCargoPassageOpen();
        GameObject.FindGameObjectsWithTag("CargoPassageControlComputer").SetLayerRecursive(LayerMask.NameToLayer("MapObject"));
        GameObject.FindGameObjectsWithTag("CargoPassageGate").SetLayerRecursive(LayerMask.NameToLayer("InteractableObject"));
    }

    public void EnablePlanC()
    {
        if (Managers.ObjectMng.MyCreature is Alien) return;

        if (!IsBatteryChargeFinished) return;

        GameObject.FindGameObjectsWithTag("EmergencyControlDevice").SetLayerRecursive(LayerMask.NameToLayer("InteractableObject"));
        Managers.ObjectMng.MyCrew.CrewIngameUI.PlanUI.EnablePlanC();
    }

    private void OnPanicRoomActivated()
    {
        if (Managers.ObjectMng.MyCreature is Alien) return;

        Managers.ObjectMng.MyCrew.CrewIngameUI.PlanUI.OnPanicRoomActivated();
        GameObject.FindGameObjectsWithTag("EmergencyControlDevice").SetLayerRecursive(LayerMask.NameToLayer("MapObject"));
        GameObject.FindGameObjectsWithTag("PanicRoom").SetLayerRecursive(LayerMask.NameToLayer("InteractableObject"));
    }
}
