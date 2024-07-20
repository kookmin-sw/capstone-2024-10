using Fusion;
using UnityEngine;

public class CargoPassageControlComputer : BaseWorkStation
{
    [SerializeField] private Gate[] _cargoPassageGates;
    protected override void Init()
    {
        base.Init();

        Description ="Open Cargo Gate";
        CrewActionType = Define.CrewActionType.KeypadUse;
        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = true;
        IsCompleted = false;

        TotalWorkAmount = 10f;
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

        if (creature is not Crew)
        {
            return false;
        }

        if (!Managers.GameMng.PlanSystem.IsCentralComputerWorkFinished)
        {
            creature.IngameUI.ErrorTextUI.Show("Use Central Computer First");
            return false;
        }

        if (Managers.GameMng.PlanSystem.IsCargoPassageOpen || IsCompleted)
        {
            creature.IngameUI.ErrorTextUI.Show("Cargo Gates are Already Open");
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        Managers.GameMng.PlanSystem.IsCargoPassageOpen = true;
        foreach (var gate in _cargoPassageGates)
        {
            gate.Open();
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/KeypadUse", 1f, 1f, isLoop: true);
    }
}
