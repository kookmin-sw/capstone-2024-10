using System.Collections;
using Fusion;
using UnityEngine;

public abstract class BaseWorkStation : NetworkBehaviour, IInteractable
{
    #region Field

    [Networked] protected float CurrentWorkAmount { get; set; }
    [Networked] protected float TotalWorkAmount { get; set; }
    [Networked] protected int WorkerCount { get; set; }
    [Networked] protected NetworkBool IsCompleted { get; set; }

    public bool CanRememberWork { get; protected set; }
    public abstract string InteractDescription { get; }

    protected Creature Worker { get; set; }
    protected Crew CrewWorker => Worker as Crew;

    #endregion

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        CurrentWorkAmount = 0f;
        IsCompleted = false;
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
        Worker.IngameUI.WorkProgressBarUI.Show(InteractDescription, CurrentWorkAmount, TotalWorkAmount);

        PlayInteractAnimation();
        Rpc_AddWorker();
        Rpc_PlayEffectMusic(Worker);
        StartCoroutine(ProgressWork());

        return true;
    }

    protected IEnumerator ProgressWork()
    {
        Worker.IngameUI.InteractInfoUI.Show("Cancel Interact");

        while (CurrentWorkAmount < TotalWorkAmount)
        while (CurrentWorkAmount < TotalWorkAmount)
        {
            if (Worker.CreatureState != Define.CreatureState.Interact)
                InterruptWork();

            Rpc_UpdateWorkAmount(Time.deltaTime, Worker.BaseStat.WorkSpeed);
            //Rpc_UpdateWorkAmount(Runner.DeltaTime, Worker.BaseStat.WorkSpeed);
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

        Rpc_StopEffectMusic();
        gameObject.GetComponent<AudioSource>().Stop();
        Rpc_RemoveWorker();
    }

    protected virtual void WorkComplete()
    {
        Rpc_WorkComplete();
        Rpc_StopEffectMusic();
    }

    protected abstract void PlayInteractAnimation();

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

    protected abstract void Rpc_PlayEffectMusic(Creature creature);

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void Rpc_StopEffectMusic()
    {
        gameObject.GetComponent<AudioSource>().Stop();
    }

    #endregion
}
