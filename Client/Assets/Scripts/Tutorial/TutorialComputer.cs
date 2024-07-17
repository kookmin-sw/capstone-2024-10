using Fusion;
using UnityEngine;

public class TutorialComputer : BaseWorkStation
{
    private new string Description => Managers.TutorialMng.TutorialPlanSystem.IsCardkeyUsed ? "Use Central Control Computer" : "Insert Card Key";

    private new Define.CrewActionType CrewActionType => Managers.GameMng.PlanSystem.IsCardkeyUsed
        ? Define.CrewActionType.KeypadUse
        : Define.CrewActionType.Insert;

    protected override void Init()
    {
        base.Init();

        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = false;
        IsCompleted = false;

        TotalWorkAmount = 2f;
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

        if (creature is not Crew crew)
        {
            return false;
        }

        if (!Managers.TutorialMng.TutorialPlanSystem.IsBatteryChargeFinished
            || Managers.TutorialMng.TutorialPlanSystem.IsComputerWorkFinished)
        {
            return false;
        }

        if (!Managers.TutorialMng.TutorialPlanSystem.IsCardkeyUsed && crew.Inventory.CurrentItem is not CardKey)
        {
            creature.IngameUI.ErrorTextUI.Show("You should have card key on your hand");
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    protected override void WorkComplete()
    {
        if (!Managers.TutorialMng.TutorialPlanSystem.IsCardkeyUsed) CrewWorker.Inventory.RemoveItem();
        base.WorkComplete();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted) return;
        if (!Managers.TutorialMng.TutorialPlanSystem.IsCardkeyUsed)
        {
            Managers.TutorialMng.TutorialPlanSystem.IsCardkeyUsed = true;
            CurrentWorkAmount = 0;
            TotalWorkAmount = 10f;
            CanRememberWork = true;
        }
        else
        {
            IsCompleted = true;
            Managers.TutorialMng.TutorialPlanSystem.IsComputerWorkFinished = true;
            Rpc_PlayCompleteSound();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        if (!Managers.TutorialMng.TutorialPlanSystem.IsCardkeyUsed) Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/Insert", 1f, 1f, isLoop: false);
        else Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/KeypadUse", 1f, 1f, isLoop: true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayCompleteSound()
    {
        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/Interactable/Plan_B", volume: 0.9f, isOneShot:true);
    }

    protected override void PlayAnim()
    {
        CrewWorker.CrewAnimController.PlayAnim(CrewActionType);
    }
}
