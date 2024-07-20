using UnityEngine;

public class TutorialCentralControlComputer : BaseWorkStation
{
    private new string Description => Managers.TutorialMng.TutorialPlanSystem.IsCardKeyUsed ? "Use Central Control Computer" : "Insert Card Key";

    private new Define.CrewActionType CrewActionType => Managers.TutorialMng.TutorialPlanSystem.IsCardKeyUsed
        ? Define.CrewActionType.KeypadUse
        : Define.CrewActionType.Insert;

    protected override void Init()
    {
        base.Init();

        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = false;
        IsCompleted = false;

        TotalWorkAmount = 1.5f;
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

        if (creature is not Crew crew)
        {
            return false;
        }

        if (!Managers.TutorialMng.TutorialPlanSystem.IsBatteryChargeFinished)
        {
            creature.IngameUI.ErrorTextUI.Show("Charge Batteries First");
            return false;
        }

        if (Managers.TutorialMng.TutorialPlanSystem.IsCentralComputerUsed)
        {
            creature.IngameUI.ErrorTextUI.Show("Already Used");
            return false;
        }

        if (!Managers.TutorialMng.TutorialPlanSystem.IsCardKeyUsed && crew.Inventory.CurrentItem is not CardKey)
        {
            creature.IngameUI.ErrorTextUI.Show("Hold Card Key on Your Hand");
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    protected override void WorkComplete()
    {
        if (!Managers.TutorialMng.TutorialPlanSystem.IsCardKeyUsed) CrewWorker.Inventory.RemoveItem();
        base.WorkComplete();
    }

    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted) return;
        if (!Managers.TutorialMng.TutorialPlanSystem.IsCardKeyUsed)
        {
            Managers.TutorialMng.TutorialPlanSystem.IsCardKeyUsed = true;
            CurrentWorkAmount = 0;
            TotalWorkAmount = 10f;
            CanRememberWork = true;
        }
        else
        {
            IsCompleted = true;
            Managers.TutorialMng.TutorialPlanSystem.IsCentralComputerUsed = true;
        }
    }

    protected override void Rpc_PlaySound()
    {
        if (!Managers.TutorialMng.TutorialPlanSystem.IsCardKeyUsed) Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/Insert", 1f, 1f, isLoop: false);
        else Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/KeypadUse", 1f, 1f, isLoop: true);
    }

    protected override void PlayAnim()
    {
        CrewWorker.CrewAnimController.PlayAnim(CrewActionType);
    }
}
