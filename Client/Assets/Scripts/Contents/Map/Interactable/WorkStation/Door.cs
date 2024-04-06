using Fusion;
using UnityEngine;

public class Door : BaseWorkStation
{
    [Networked] private NetworkBool IsOpened { get; set; }
    public NetworkMecanimAnimator NetworkAnim { get; protected set; }
    public override string InteractDescription => IsOpened ? "Close Door" : "Open Door";

    protected override void Init()
    {
        base.Init();

        NetworkAnim = transform.GetComponent<NetworkMecanimAnimator>();

        IsOpened = false;
        _canRememberWork = false;

        TotalWorkAmount = 5f; // only for alien crashing door
    }

    public override bool TryShowInfoUI(Creature creature, out bool isInteractable)
    {
        isInteractable = false;
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

        if (creature is Alien && IsOpened) return false;

        return true;
    }

    public override bool Interact(Creature creature)
    {
        if (!IsInteractable(creature)) return false;

        Worker = creature;
        Worker.IngameUI.InteractInfoUI.Hide();
        Worker.CreatureState = Define.CreatureState.Interact;
        Worker.CreaturePose = Define.CreaturePose.Stand;

        PlayInteractAnimation();
        Rpc_AddWorker();

        if (creature is Crew)
        {
            InterruptWork();
            WorkComplete();
        }
        else
        {
            Worker.IngameUI.WorkProgressBarUI.Show(InteractDescription, CurrentWorkAmount, TotalWorkAmount);
            StartCoroutine(ProgressWork());
        }

        return true;
    }

    protected override void WorkComplete()
    {
        switch (Worker)
        {
            case Crew:
                Rpc_WorkComplete();
                break;
            case Alien:
                Rpc_AlienWorkComplete();
                break;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        IsOpened = !IsOpened;
        NetworkAnim.Animator.SetBool("OpenParameter", IsOpened);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_AlienWorkComplete()
    {
        gameObject.SetActive(false);
        //Managers.NetworkMng.Runner.Despawn(gameObject.GetComponent<NetworkObject>());
    }

    protected override void PlayInteractAnimation()
    {
        if (IsOpened) return;

        switch (Worker)
        {
            case Crew crew:
                crew.CrewAnimController.PlayOpenDoor();
                break;
            case Alien alien:
                alien.AlienAnimController.PlayCrashDoor();
                break;
        }
    }
}
