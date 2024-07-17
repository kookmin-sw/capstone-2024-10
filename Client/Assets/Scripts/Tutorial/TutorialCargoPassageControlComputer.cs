using UnityEngine;

public class TutorialCargoPassageControlComputer : BaseWorkStation
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

        if (!Managers.TutorialMng.TutorialPlanSystem.IsCentralComputerUsed)
        {
            creature.IngameUI.ErrorTextUI.Show("Use Central Computer First");
            return false;
        }

        if (Managers.TutorialMng.TutorialPlanSystem.IsCargoGateOpen || IsCompleted)
        {
            creature.IngameUI.ErrorTextUI.Show("Cargo Gates are Already Open");
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        Managers.TutorialMng.TutorialPlanSystem.IsCargoGateOpen = true;

        foreach (var gate in _cargoPassageGates)
        {
            gate.gameObject.SetLayerRecursive(LayerMask.NameToLayer("InteractableObject"));
            gate.Open();
        }

        Rpc_PlayCompleteSound();
    }

    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/KeypadUse", 1f, 1f, isLoop: true);
    }

    protected void Rpc_PlayCompleteSound()
    {
        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/Interactable/Plan_B", volume: 0.9f, isOneShot:true);
    }
}
