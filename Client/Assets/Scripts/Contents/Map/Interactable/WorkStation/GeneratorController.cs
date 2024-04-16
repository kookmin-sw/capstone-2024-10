using Fusion;
using UnityEngine;

public class GeneratorController : BaseWorkStation
{
    public override string InteractDescription => "RESTORE GENERATOR";
    public AudioSource AudioSource { get; protected set; }
    protected override void Init()
    {
        base.Init();
        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = true;
        IsCompleted = false;

        TotalWorkAmount = 50f;
    }
    public override bool CheckInteractable(Creature creature)
    {
        if (creature is not Crew crew)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Hide();
            return false;
        }

        if (creature.CreatureState == Define.CreatureState.Interact)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Hide();
            return false;
        }

        if (Managers.MapMng.PlanSystem.IsGeneratorRestored)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("COMPLETED");
            return true;
        }

        if (!Managers.MapMng.PlanSystem.IsBatteryChargeFinished)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("CAN NOT USE NOW");
            return true;
        }

        creature.IngameUI.ErrorTextUI.Hide();
        creature.IngameUI.InteractInfoUI.Show(InteractDescription);
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
        Managers.MapMng.PlanSystem.IsGeneratorRestored = true;
    }

    protected override void PlayInteractAnimation()
    {
        CrewWorker.CrewAnimController.PlayKeypadUse();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlayEffectMusic(Creature creature)
    {
        AudioSource.volume = 1f;
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip("Music/Clicks/GeneratorController");
        AudioSource.Play();

        //Managers.SoundMng.Play("Music/Clicks/GeneratorController", Define.SoundType.Effect, 1f);
    }
}
