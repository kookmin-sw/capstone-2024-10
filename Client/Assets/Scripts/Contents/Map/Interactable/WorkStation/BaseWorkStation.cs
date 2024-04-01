using System.Collections;
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

    [Networked, Capacity(3)] public NetworkLinkedList<NetworkId> CurrentWorkers { get; }
    public Creature MyWorker { get; protected set; }

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        IsCompleted = false;
        CurrentWorkAmount = 0f;
    }

    public override bool CheckAndInteract(Creature creature)
    {
        if (creature.CreatureType == Define.CreatureType.Alien)
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
        MyWorker.IngameUI.WorkProgressBarUI.Show(InteractDescription.ToString(), TotalWorkAmount);

        Rpc_AddWorker(MyWorker.NetworkObject.Id);
        PlayInteract();

        StartCoroutine(CoWorkProgress());

        return true;
    }

    public void MyWorkInterrupt()
    {
        StopAllCoroutines();

        MyWorker.InterruptInteract();
        Rpc_MyWorkInterrupt(MyWorker.NetworkObject.Id);

        Debug.Log($"{MyWorker.NetworkObject.Id}: Interrupt Work"); // TODO - Test code
    }

    public virtual void WorkComplete()
    {
        if (MyWorker.CreatureType == Define.CreatureType.Crew)
            Rpc_CrewWorkComplete();
        else if (MyWorker.CreatureType == Define.CreatureType.Alien)
            Rpc_AlienWorkComplete();
        else
            Debug.LogError("Failed to WorkComplete");
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
    public virtual void Rpc_WorkProgress(float workSpeed)
    {
        CurrentWorkAmount = Mathf.Clamp(CurrentWorkAmount + Time.deltaTime * workSpeed, 0, TotalWorkAmount);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected virtual void Rpc_CrewWorkComplete()
    {
        IsCompleted = true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected virtual void Rpc_AlienWorkComplete()
    {
        IsCompleted = true;
    }

    protected virtual IEnumerator CoWorkProgress()
    {
        while (CurrentWorkAmount < TotalWorkAmount)
        {
            if (MyWorker.CreatureState != Define.CreatureState.Interact)
                MyWorkInterrupt();

            Rpc_WorkProgress(MyWorker.CreatureData.WorkSpeed);
            MyWorker.IngameUI.WorkProgressBarUI.CurrentWorkAmount = CurrentWorkAmount;

            yield return null;
        }

        MyWorkInterrupt();
        WorkComplete();
    }
}
