using Fusion;

public class Door : BaseWorkStation
{
    [Networked] public NetworkBool IsOpened { get; set; }

    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    public override string InteractDescription => IsOpened ? "Close door" : "Open door"; 

    protected override void Init()
    {
        base.Init();

        NetworkAnim = transform.GetComponent<NetworkMecanimAnimator>();

        CanUseAgain = true;
        CanCollaborate = false;
        CanRememberWork = false;
        IsCompleted = false;
        IsOpened = false;

        TotalWorkAmount = 5f;
        WorkingDescription = "DOOR";
    }

    public override bool CheckAndInteract(Creature creature)
    {
        if (!CanUseAgain && IsCompleted)
            return false;

        if (CurrentWorkers.Count >= 3 || (!CanCollaborate && CurrentWorkers.Count >= 1))
            return false;

        if (!(creature.CreatureState == Define.CreatureState.Idle || creature.CreatureState == Define.CreatureState.Move))
            return false;

        if (creature.CreatureType == Define.CreatureType.Alien && IsOpened)
            return false;

        return true;
    }

    public override void StartInteract(Creature creature)
    {
        MyWorker = creature;
        Rpc_AddWorker(MyWorker.NetworkObject.Id);
        PlayInteract();

        if (IsOpened)
            TotalWorkAmount = 1;
        else
            TotalWorkAmount = 5;

        if (MyWorker.CreatureType == Define.CreatureType.Crew)
            ((Crew)MyWorker).CrewInGameUI.ShowWorkProgressBar(Description.ToString(), TotalWorkAmount);

        StartCoroutine(CoWorkProgress());
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_CrewWorkComplete()
    {
        IsOpened = !IsOpened;
        NetworkAnim.Animator.SetBool("OpenParameter", IsOpened);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_AlienWorkComplete()
    {
        gameObject.SetActive(false);
        //Managers.NetworkMng.Runner.Despawn(gameObject.GetComponent<NetworkObject>());
    }

    public override void PlayInteract()
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
