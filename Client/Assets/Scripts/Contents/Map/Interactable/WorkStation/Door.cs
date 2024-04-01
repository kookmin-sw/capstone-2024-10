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

    public override bool CheckAndInteract(Creature creature)
    {
        if (creature.CreatureType == Define.CreatureType.Alien && IsOpened)
            return false;

        if (!CanUseAgain && IsCompleted)
            return false;

        if (CurrentWorkers.Count >= 3 || (!CanCollaborate && CurrentWorkers.Count >= 1))
            return false;

        if (!(creature.CreatureState == Define.CreatureState.Idle || creature.CreatureState == Define.CreatureState.Move))
            return false;

        MyWorker = creature;
        MyWorker.IngameUI.InteractInfoUI.Hide();
        MyWorker.CreatureState = Define.CreatureState.Interact;
        MyWorker.CreaturePose = Define.CreaturePose.Stand;
        MyWorker.CurrentWorkStation = this;

        TotalWorkAmount = IsOpened ? CloseWorkAmount : OpenWorkAmount;
        MyWorker.IngameUI.WorkProgressBarUI.Show(InteractDescription.ToString(), TotalWorkAmount);

        Rpc_AddWorker(MyWorker.NetworkObject.Id);
        PlayInteract();

        StartCoroutine(CoWorkProgress());

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
