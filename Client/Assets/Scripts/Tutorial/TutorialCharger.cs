using Fusion;

public class TutorialCharger : BatteryCharger
{
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        CurrentWorkAmount = 0;
        Managers.TutorialMng.TutorialPlanSystem.BatteryChargeCount++;

        if (Managers.TutorialMng.TutorialPlanSystem.BatteryChargeCount == 2)
        {
            Rpc_PlayCompleteSound();
        }
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

        if (Managers.TutorialMng.TutorialPlanSystem.IsBatteryChargeFinished)
        {
            creature.IngameUI.ErrorTextUI.Show("Battery Charge Completed");
            return false;
        }

        return true;
    }
}
