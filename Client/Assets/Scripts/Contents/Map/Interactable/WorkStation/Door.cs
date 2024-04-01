using Fusion;

public class Door : BaseWorkStation
{
    [Networked] public float OpenWorkAmount { get; set; }
    [Networked] public float CloseWorkAmount { get; set; }

    [Networked] public NetworkBool IsOpened { get; set; }

    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    protected override void Init()
    {
        base.Init();

        NetworkAnim = transform.GetComponent<NetworkMecanimAnimator>();

        InteractDescription = "USE DOOR";

        CanUseAgain = true;
        CanCollaborate = false;
        CanRememberWork = false;
        IsCompleted = false;
        IsOpened = false;

        OpenWorkAmount = 5;
        CloseWorkAmount = 1;

        TotalWorkAmount = IsOpened ? CloseWorkAmount : OpenWorkAmount;
    }

    public override bool IsInteractable(Creature creature, bool isDoInteract)
    {
        if (creature.CreatureType == Define.CreatureType.Alien && IsOpened)
            return false;

        if (!CanUseAgain && IsCompleted)
            return false;

        creature.IngameUI.InteractInfoUI.Show(InteractDescription.ToString());

        if (CurrentWorkers.Count >= 3 || (!CanCollaborate && CurrentWorkers.Count >= 1))
            return false;

        if (!(creature.CreatureState == Define.CreatureState.Idle || creature.CreatureState == Define.CreatureState.Move))
            return false;

        TotalWorkAmount = IsOpened ? CloseWorkAmount : OpenWorkAmount;

        if (isDoInteract)
            Interact(creature);

        return true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_CrewWorkComplete()
    {
        base.Rpc_CrewWorkComplete();

        IsOpened = !IsOpened;
        NetworkAnim.Animator.SetBool("OpenParameter", IsOpened);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_AlienWorkComplete()
    {
        base.Rpc_CrewWorkComplete();

        gameObject.SetActive(false);
        //Managers.NetworkMng.Runner.Despawn(gameObject.GetComponent<NetworkObject>());
    }

    public override void PlayInteractAnimation()
    {
        if (!IsOpened)
        {
            if (MyWorker.CreatureType == Define.CreatureType.Crew)
                ((Crew)MyWorker).CrewAnimController.PlayOpenDoor();
            else
                ((Alien)MyWorker).AlienAnimController.PlayCrashDoor();
        }
    }
}
