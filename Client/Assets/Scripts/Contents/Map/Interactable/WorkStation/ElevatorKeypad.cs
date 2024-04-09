using Fusion;
using UnityEngine;

public class ElevatorKeypad : BaseWorkStation
{
    [SerializeField] private GameObject _elevatorDoor;
    public override string InteractDescription => "Activate the elevator";

    protected override void Init()
    {
        base.Init();

        _canRememberWork = true;
        IsCompleted = false;

        TotalWorkAmount = 50f;
    }
    public override bool TryShowInfoUI(Creature creature, out bool isInteractable)
    {
        isInteractable = false;
        if (creature.CreatureType == Define.CreatureType.Alien)
            return false;

        if (IsCompleted)
        {
            return false;
        }

        if (!Managers.MapMng.PlanSystem.IsGeneratorRestored)
        {
            creature.IngameUI.ErrorTextUI.Show("You cannot use this now");
        }

        if (creature.CreatureState == Define.CreatureState.Interact)
            return false;

        creature.IngameUI.ErrorTextUI.Hide();
        creature.IngameUI.InteractInfoUI.Show(InteractDescription);
        isInteractable = true;
        return true;
    }

    protected override bool IsInteractable(Creature creature)
    {
        if (WorkerCount > 0) return false;

        return true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        _elevatorDoor.GetComponent<NetworkMecanimAnimator>().Animator.SetBool("OpenParameter", true);
    }

    protected override void PlayInteractAnimation()
    {
        CrewWorker.CrewAnimController.PlayKeypadUse();
    }
}
