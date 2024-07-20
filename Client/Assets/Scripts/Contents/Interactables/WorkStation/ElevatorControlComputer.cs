using Fusion;
using UnityEngine;

public class ElevatorControlComputer : BaseWorkStation
{
    protected override void Init()
    {
        base.Init();

        Description ="Insert USB Key";
        CrewActionType = Define.CrewActionType.Insert;
        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = false;
        IsCompleted = false;

        TotalWorkAmount = 1.5f;
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

        if (creature is not Crew crew)
        {
            return false;
        }

        if (!Managers.GameMng.PlanSystem.IsBatteryChargeFinished)
        {
            creature.IngameUI.ErrorTextUI.Show("Charge Batteries First");
            return false;
        }

        if (Managers.GameMng.PlanSystem.IsUSBKeyInsertFinished)
        {
            creature.IngameUI.ErrorTextUI.Show("Elevators are Already Activated");
            return false;
        }

        if (crew.Inventory.CurrentItem is not USBKey)
        {
            creature.IngameUI.ErrorTextUI.Show("Hold USB Key on Your Hand");
            return false;
        }

        if (WorkerCount > 0 && Worker == null)
        {
            creature.IngameUI.ErrorTextUI.Show("Another player is in Use");
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);

        return true;
    }

    protected override void WorkComplete()
    {
        CrewWorker.Inventory.RemoveItem();
        base.WorkComplete();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        CurrentWorkAmount = 0;
        Managers.GameMng.PlanSystem.USBKeyInsertCount++;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/Insert", 1f, 1f, isLoop: false);
    }
}
