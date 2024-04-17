using Fusion;
using UnityEngine;

public class Computer : BaseWorkStation
{
    public override string InteractDescription => "USE COMPUTER";
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

        if (IsCompleted)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("COMPLETED");
            return false;
        }

        creature.IngameUI.ErrorTextUI.Hide();
        creature.IngameUI.InteractInfoUI.Show(InteractDescription);
        return true;
    }

    protected override bool IsInteractable(Creature creature)
    {
        return true;
    }

    protected override void PlayInteractAnimation()
    {
        CrewWorker.CrewAnimController.PlayKeypadUse();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted) return;
        IsCompleted = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlayEffectMusic(Creature creature)
    {
        AudioSource.volume = 1f;
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip("Music/Clicks/Typing_Keyboard");
        AudioSource.loop = true;
        AudioSource.Play();
        //Managers.SoundMng.Play("Music/Clicks/Typing_Keyboard", Define.SoundType.Effect, 1f, true);
    }

}
