using Fusion;
using UnityEngine;

public abstract class BaseWorkStation : BaseInteractable
{
    [Networked] public float TotalWorkAmount { get; set; }
    [Networked] public float CurrentWorkAmount { get; set; }
    [Networked] public NetworkBool CanUseAgain { get; set; }
    [Networked] public NetworkBool CanRememberWork { get; set; }
    [Networked] public NetworkBool CanCollaborate { get; set; }
    [Networked] public NetworkBool IsCompleted { get; set; }
    [Networked] public NetworkString<_16> Description { get; set; }

    [Networked, Capacity(3)] public NetworkLinkedList<NetworkId> CurrentWorkers { get; }
    public Creature MyWorker => Managers.ObjectMng.MyCreature;

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        CurrentWorkAmount = 0f;
    }

    public override bool CheckAndInteract(Creature creature)
    {
        if (CanUseAgain)
            IsCompleted = false;

        if (IsCompleted || CurrentWorkers.Count >= 3 || (!CanCollaborate && CurrentWorkers.Count >= 1))
            return false;

        Rpc_AddWorker(MyWorker.NetworkObject.Id);
        PlayInteract();

        return true;
    }

    public void MyWorkInterrupt()
    {
        StopAllCoroutines();

        Rpc_MyWorkInterrupt(MyWorker.NetworkObject.Id);

        Debug.Log($"{MyWorker.NetworkObject.Id}: Interrupt Work"); // TODO - Test code
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected void Rpc_AddWorker(NetworkId networkId)
    {
        CurrentWorkers.Add(networkId);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected void Rpc_MyWorkInterrupt(NetworkId networkId)
    {
        CurrentWorkers.Remove(networkId);

        if (!CanRememberWork && CurrentWorkers.Count <= 0)
            CurrentWorkAmount = 0f;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected virtual void Rpc_WorkComplete()
    {
        IsCompleted = true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void Rpc_WorkProgress(float workSpeed)
    {
        CurrentWorkAmount = Mathf.Clamp(CurrentWorkAmount + Time.deltaTime * workSpeed, 0, TotalWorkAmount);

        if (CurrentWorkAmount >= TotalWorkAmount)
            Rpc_WorkComplete();
    }
}
