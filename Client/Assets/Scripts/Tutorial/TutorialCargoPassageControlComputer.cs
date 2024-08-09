using UnityEngine;

public class TutorialCargoPassageControlComputer : BaseWorkStation
{
    [SerializeField] private Gate[] _cargoPassageGates;
    protected override void Init()
    {
        base.Init();

        Description = "Open Cargo Passage Gate";
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
            return false;
        }

        if (Managers.TutorialMng.TutorialPlanSystem.IsCargoGateOpen || IsCompleted)
        {
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
            gate.gameObject.SetLayerRecursive(LayerMask.NameToLayer("PlanTargetObject"));
            gate.Open();
        }
    }

    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/KeypadUse", 1f, 1f, isLoop: true);
    }
}
