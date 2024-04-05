using UnityEngine;
using Fusion;

public class BatteryCharger : BaseWorkStation
{
    public override string InteractDescription => "Charge battery";

    protected override void Init()
    {
        base.Init();
        
        _canRememberWork = false;
        IsCompleted = false;

        TotalWorkAmount = 10f;
    }
    public override bool TryShowInfoUI(Creature creature, out bool isInteractable)
    {
        isInteractable = false;
        if (creature.CreatureType == Define.CreatureType.Alien)
            return false;

        if (creature.CreatureState == Define.CreatureState.Interact)
            return false;

        if (!((Crew)creature).Inventory.HasItem(Define.ITEM_Battery_ID))
        {
            creature.IngameUI.ErrorTextUI.Show("You don't have any battery");
            return true;
        }

        if (Managers.MapMng.MapSystem.BatteryCollectFinished)
        {
            creature.IngameUI.ErrorTextUI.Show("All batteries are already charged");
            return true;
        }

        creature.IngameUI.InteractInfoUI.Show(InteractDescription);
        isInteractable = true;
        return true;
    }

    protected override bool IsInteractable(Creature creature)
    {
        if (WorkerCount > 0) return false;

        return true;
    }

    protected override void WorkComplete()
    {
        CrewWorker.Inventory.RemoveItem(Define.ITEM_Battery_ID);

        base.WorkComplete();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        CurrentWorkAmount = 0;
        Managers.MapMng.MapSystem.BatteryCollectCount++;
    }
    protected override void PlayInteractAnimation()
    {
        CrewWorker.CrewAnimController.PlayKeypadUse();
    }
}

