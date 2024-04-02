using UnityEngine;
using Fusion;

public class BatteryCharger : BaseWorkStation
{
    public Crew MyCrew => (Crew)MyWorker;

    protected override void Init()
    {
        base.Init();

        InteractDescription = "Charge battery";

        CanUseAgain = true;
        CanRememberWork = true;
        CanCollaborate = true;
        IsCompleted = false;

        TotalWorkAmount = 10f;
    }

    public override bool IsInteractable(Creature creature, bool isDoInteract)
    {
        if (creature.CreatureType == Define.CreatureType.Alien)
            return false;

        if (!CanUseAgain && IsCompleted)
            return false;

        if (Managers.MapMng.MapSystem.BatteryCollectFinished)
            return false;

        creature.IngameUI.InteractInfoUI.Show(InteractDescription.ToString());

        if (CurrentWorkers.Count >= 3 || (!CanCollaborate && CurrentWorkers.Count >= 1))
            return false;

        if (!(creature.CreatureState == Define.CreatureState.Idle || creature.CreatureState == Define.CreatureState.Move))
            return false;

        if (!((Crew)creature).Inventory.HasItem(Define.ITEM_Battery_ID))
            return false;

        if (isDoInteract)
            Interact(creature);

        return true;
    }

    public override void WorkComplete()
    {
        MyCrew.Inventory.RemoveItem(Define.ITEM_Battery_ID);

        base.WorkComplete();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_CrewWorkComplete()
    {
        CurrentWorkAmount = 0;
        Managers.MapMng.MapSystem.BatteryCollectCount++;
    }
    public override void PlayInteractAnimation()
    {
        MyCrew.CrewAnimController.PlayKeypadUse();
    }
}

