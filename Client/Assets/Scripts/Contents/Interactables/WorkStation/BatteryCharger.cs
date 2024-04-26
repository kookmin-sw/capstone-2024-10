using Fusion;
using UnityEngine;

public class BatteryCharger : BaseWorkStation
{
    protected override void Init()
    {
        base.Init();

        Description = "Charge Battery";
        CrewActionType = Define.CrewActionType.ChargeBattery;
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

        if (Managers.GameMng.PlanSystem.IsBatteryChargeFinished)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("Charge Finished");
            return false;
        }

        if (!(crew.Inventory.CurrentItem is Battery))
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("No Battery On Your Hand");
            return false;
        }

        creature.IngameUI.ErrorTextUI.Hide();
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
        Managers.GameMng.PlanSystem.BatteryChargeCount++;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Interactable/BatteryCharger");
        AudioSource.volume = 1f;
        AudioSource.loop = false;
        AudioSource.Play();
    }
}

