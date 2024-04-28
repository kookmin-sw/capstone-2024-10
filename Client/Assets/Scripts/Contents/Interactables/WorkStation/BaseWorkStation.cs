using System.Collections;
using Fusion;
using UnityEngine;

public abstract class BaseWorkStation : NetworkBehaviour, IInteractable
{
    #region Field

    public string Description { get; protected set; }
    public Define.CrewActionType CrewActionType { get; protected set; }
    [Networked] protected float CurrentWorkAmount { get; set; } = 0f;
    [Networked] protected float TotalWorkAmount { get; set; }
    [Networked] protected int WorkerCount { get; set; }
    [Networked] protected NetworkBool IsCompleted { get; set; } = false;

    public bool CanRememberWork { get; protected set; }

    public AudioSource AudioSource { get; protected set; }

    protected Creature Worker { get; set; }
    protected Crew CrewWorker => Worker as Crew;

    #endregion

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    #region Interact

    public abstract bool CheckInteractable(Creature creature);

    protected virtual bool IsInteractable(Creature creature)
    {
        if (WorkerCount > 0)
            return false;

        return true;
    }

    public virtual bool Interact(Creature creature)
    {
        if (!IsInteractable(creature))
            return false;

        Worker = creature;
        Worker.IngameUI.InteractInfoUI.Hide();
        Worker.CreatureState = Define.CreatureState.Interact;
        Worker.CreaturePose = Define.CreaturePose.Stand;
        Worker.IngameUI.WorkProgressBarUI.Show(Description, CurrentWorkAmount, TotalWorkAmount);

        Rpc_AddWorker();
        PlayAnim();
        Rpc_PlaySound();
        StartCoroutine(ProgressWork());

        return true;
    }

    protected IEnumerator ProgressWork()
    {
        Worker.IngameUI.InteractInfoUI.Show("Cancel Interact");

        while (CurrentWorkAmount < TotalWorkAmount)
        {
            if (Worker.CreatureState != Define.CreatureState.Interact)
                InterruptWork();

            Rpc_UpdateWorkAmount(Time.deltaTime, Worker.BaseStat.WorkSpeed);
            Worker.IngameUI.WorkProgressBarUI.CurrentWorkAmount = CurrentWorkAmount;
            yield return null;
        }

        Worker.IngameUI.InteractInfoUI.Hide();

        InterruptWork();
        WorkComplete();
    }

    protected void InterruptWork()
    {
        StopAllCoroutines();
        Worker.IngameUI.WorkProgressBarUI.Hide();
        Worker.ReturnToIdle(0f);

        Rpc_RemoveWorker();
        Rpc_StopSound();
    }

    protected virtual void WorkComplete()
    {
        Rpc_WorkComplete();
    }

    protected virtual void PlayAnim()
    {
        CrewWorker.CrewAnimController.PlayAnim(CrewActionType);
    }

    #endregion

    #region Rpc

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void Rpc_UpdateWorkAmount(float deltaTime, float workSpeed)
    {
        CurrentWorkAmount = Mathf.Clamp(CurrentWorkAmount + deltaTime * workSpeed, 0, TotalWorkAmount);
    }

    protected abstract void Rpc_WorkComplete();

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected void Rpc_AddWorker()
    {
        WorkerCount++;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected void Rpc_RemoveWorker()
    {
        WorkerCount--;

        if (!CanRememberWork && WorkerCount <= 0)
            CurrentWorkAmount = 0f;
    }

    protected abstract void Rpc_PlaySound();

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_StopSound()
    {
        AudioSource.Stop();
    }

    #endregion
}
