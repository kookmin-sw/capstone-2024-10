using Fusion;
using UnityEngine;

public class CentralControlComputer : BaseWorkStation
{
    protected override void Init()
    {
        base.Init();

        Description ="Use keycard on computer";
        CrewActionType = Define.CrewActionType.KeypadUse;
        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = true;
        IsCompleted = false;

        //TotalWorkAmount = 150f;
        TotalWorkAmount = 15f; // TODO: for test
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

        if (creature is not Crew crew)
        {
            return false;
        }

        if (Managers.GameMng.PlanSystem.IsKeycardUsed || !Managers.GameMng.PlanSystem.IsBatteryChargeFinished)
        {
            return false;
        }

        if (crew.Inventory.CurrentItem is not CardKey)
        {
            creature.IngameUI.ErrorTextUI.Show("You should have cardkey on your hand");
            return false;
        }

        creature.IngameUI.ErrorTextUI.Hide();
        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        Managers.GameMng.PlanSystem.IsKeycardUsed = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/GeneratorController", 1f, 1f, isLoop: true);
    }
}
