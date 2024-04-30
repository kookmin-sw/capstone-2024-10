using Fusion;
using UnityEngine;

public class ElevatorKeypad : BaseWorkStation
{
    [SerializeField] private GameObject _elevatorDoor;

    protected override void Init()
    {
        base.Init();

        Description = "Activate Elevator";
        CrewActionType = Define.CrewActionType.KeypadUse;
        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = true;
        IsCompleted = false;

        TotalWorkAmount = 60f;
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

        if (IsCompleted)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("Completed");
            return false;
        }

        if (!Managers.GameMng.PlanSystem.IsGeneratorRestored)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("Not Ready");
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
        _elevatorDoor.GetComponent<NetworkMecanimAnimator>().Animator.SetBool("OpenParameter", true);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/KeypadUse", 1f, 1f, isLoop: true);
    }
}
