using Fusion;

public class GeneratorController : BaseWorkStation
{
    public override string InteractDescription => "Restore generator";

    protected override void Init()
    {
        base.Init();

        _canRememberWork = true;
        IsCompleted = false;

        TotalWorkAmount = 50f;
    }
    public override bool TryShowInfoUI(Creature creature, out bool isInteractable)
    {
        isInteractable = false;
        if (creature.CreatureType == Define.CreatureType.Alien)
            return false;

        if (Managers.MapMng.MapSystem.IsGeneratorRestored)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("The generator has already been restored");
            return true;
        }

        if (!Managers.MapMng.MapSystem.IsBatteryChargeFinished)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("You cannot use this now");
            return true;
        }

        if (creature.CreatureState == Define.CreatureState.Interact)
            return false;

        creature.IngameUI.ErrorTextUI.Hide();
        creature.IngameUI.InteractInfoUI.Show(InteractDescription);
        isInteractable = true;
        return true;
    }

    protected override bool IsInteractable(Creature creature)
    {
        return true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        Managers.MapMng.MapSystem.IsGeneratorRestored = true;
    }

    protected override void PlayInteractAnimation()
    {
        CrewWorker.CrewAnimController.PlayKeypadUse();
    }
}
