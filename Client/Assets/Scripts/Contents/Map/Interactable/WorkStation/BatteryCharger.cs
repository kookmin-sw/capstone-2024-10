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

        if (!(((Crew)creature).Inventory.CurrentItem is Battery))
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("You should have a battery on your hand");
            return true;
        }

        if (Managers.MapMng.PlanSystem.IsBatteryChargeFinished)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("All batteries are charged already");
            return true;
        }

        creature.IngameUI.ErrorTextUI.Hide();
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
        CrewWorker.Inventory.RemoveItem();

        base.WorkComplete();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        CurrentWorkAmount = 0;
        Managers.MapMng.PlanSystem.BatteryChargeCount++;
    }
    protected override void PlayInteractAnimation()
    {
        CrewWorker.CrewAnimController.PlayChargeBattery();
    }

    protected override void PlayEffectMusic()
    {
        
    }
}

