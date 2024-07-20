using Fusion;
using UnityEngine;

public class TutorialCharger : BaseWorkStation
{
    protected override void Init()
    {
        base.Init();

        Description = "Charge Battery";
        CrewActionType = Define.CrewActionType.ChargeBattery;
        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = false;
        IsCompleted = false;

        TotalWorkAmount = 1.8f;
    }

    public override bool IsInteractable(Creature creature)
    {
        if(!base.IsInteractable(creature)) return false;

        if (creature is not Crew crew)
        {
            return false;
        }

        if (Managers.TutorialMng.TutorialPlanSystem.IsBatteryChargeFinished)
        {
            creature.IngameUI.ErrorTextUI.Show("Battery Charge Completed");
            return false;
        }

        if (WorkerCount > 0 && Worker == null)
        {
            creature.IngameUI.ErrorTextUI.Show("Another Crew is in Use");
            return false;
        }

        if (crew.Inventory.CurrentItem is not Battery)
        {
            creature.IngameUI.ErrorTextUI.Show("Hold Battery on Your Hand");
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    protected override void WorkComplete()
    {
        CrewWorker.Inventory.RemoveItem();
        base.WorkComplete();
    }

    protected override void Rpc_WorkComplete()
    {
        CurrentWorkAmount = 0;
        Managers.TutorialMng.TutorialPlanSystem.BatteryChargeCount++;
    }

    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/BatteryCharger", 1f, 1f, isLoop: false);
    }
}
