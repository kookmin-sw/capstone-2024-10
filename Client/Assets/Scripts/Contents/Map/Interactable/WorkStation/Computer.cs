using Fusion;


public class Computer : BaseWorkStation
{
    public override string InteractDescription => "Use computer";

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

        if (IsCompleted)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("This computer has already been completely used");
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
    }

    protected override void PlayInteractAnimation()
    {
        CrewWorker.CrewAnimController.PlayKeypadUse();
    }

    protected override void PlayEffectMusic()
    {
        Managers.SoundMng.Play("Music/Clicks/Typing_Keyboard", Define.SoundType.Effect, 0.5f, true);
    }

    
}
