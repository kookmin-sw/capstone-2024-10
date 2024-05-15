using Fusion;
using UnityEngine;

public class CentralControlComputer : BaseWorkStation
{
    private new string Description => Managers.GameMng.PlanSystem.IsCardkeyUsed ? "Use Central Control Computer" : "Insert Card Key";

    private new Define.CrewActionType CrewActionType => Managers.GameMng.PlanSystem.IsCardkeyUsed
        ? Define.CrewActionType.KeypadUse
        : Define.CrewActionType.Insert;

    protected override void Init()
    {
        base.Init();

        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = false;
        IsCompleted = false;

        TotalWorkAmount = 20f;
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

        if (creature is not Crew crew)
        {
            return false;
        }

        if (!Managers.GameMng.PlanSystem.IsBatteryChargeFinished || Managers.GameMng.PlanSystem.IsCentralComputerWorkFinished)
        {
            return false;
        }

        if (!Managers.GameMng.PlanSystem.IsCardkeyUsed && crew.Inventory.CurrentItem is not CardKey)
        {
            creature.IngameUI.ErrorTextUI.Show("You should have card key on your hand");
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    protected override void WorkComplete()
    {
        if(!Managers.GameMng.PlanSystem.IsCardkeyUsed) CrewWorker.Inventory.RemoveItem();
        base.WorkComplete();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted) return;
        if (!Managers.GameMng.PlanSystem.IsCardkeyUsed)
        {
            Managers.GameMng.PlanSystem.IsCardkeyUsed = true;
            CurrentWorkAmount = 0;
            TotalWorkAmount = 150f;
            CanRememberWork = true;
        }
        else
        {
            IsCompleted = true;
            Managers.GameMng.PlanSystem.IsCentralComputerWorkFinished = true;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        if (!Managers.GameMng.PlanSystem.IsCardkeyUsed) Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/Insert", 1f, 1f, isLoop: false);
        else Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/KeypadUse", 1f, 1f, isLoop: true);
    }

    protected override void PlayAnim()
    {
        CrewWorker.CrewAnimController.PlayAnim(CrewActionType);
    }
}
