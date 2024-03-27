using Fusion;

public class Door : BaseWorkStation
{
    public Crew MyCrew => (Crew)MyWorker;

    [Networked] public NetworkBool IsOpened { get; set; }

    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    protected override void Init()
    {
        base.Init();

        NetworkAnim = transform.GetComponent<NetworkMecanimAnimator>();

        CanUseAgain = true;
        CanCollaborate = false;
        CanRememberWork = false;
        IsCompleted = false;

        TotalWorkAmount = 5f;
        Description = "DOOR";
    }

    public override bool CheckAndInteract(Creature creature)
    {
        if (IsOpened)
            TotalWorkAmount = 1;
        else
            TotalWorkAmount = 5;

        if (CanUseAgain)
            IsCompleted = false;

        if (IsCompleted || CurrentWorkers.Count >= 3 || (!CanCollaborate && CurrentWorkers.Count >= 1))
            return false;

        Rpc_AddWorker(MyWorker.NetworkObject.Id);
        PlayInteract();

        return true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        base.Rpc_WorkComplete();

        IsOpened = !IsOpened;
        NetworkAnim.Animator.SetBool("OpenParameter", IsOpened);
    }

    public override void PlayInteract()
    {
        if (!IsOpened)
            MyCrew.CrewAnimController.PlayOpenDoor();
    }
}
