using Fusion;
using UnityEngine;

public class BatteryCharger : BaseWorkStation
{
    public override string InteractDescription => "CHARGE BATTERY";
    public AudioSource AudioSource { get; protected set; }

    protected override void Init()
    {
        base.Init();

        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = false;
        IsCompleted = false;

        TotalWorkAmount = 15f;
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

        if (Managers.MapMng.PlanSystem.IsBatteryChargeFinished)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("CHARGE FINISHED");
            return false;
        }

        if (!(crew.Inventory.CurrentItem is Battery))
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("NO BATTERY ON YOUR HAND");
            return false;
        }

        creature.IngameUI.ErrorTextUI.Hide();
        creature.IngameUI.InteractInfoUI.Show(InteractDescription);
        return true;
    }

    protected override void WorkComplete()
    {
        CrewWorker.Inventory.RemoveItem();
        base.WorkComplete();
    }

    protected override void PlayInteractAnimation()
    {
        CrewWorker.CrewAnimController.PlayChargeBattery();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        CurrentWorkAmount = 0;
        Managers.MapMng.PlanSystem.BatteryChargeCount++;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlayEffectMusic(Creature creature)
    {
        AudioSource.volume = 1f;
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip("Music/Clicks/BatteryCharger");
        AudioSource.Play();
        //Managers.SoundMng.Play("Music/Clicks/BatteryCharger", Define.SoundType.Effect, 1f);
    }
}

