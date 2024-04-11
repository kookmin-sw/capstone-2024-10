using System.Collections;
using Fusion;
using UnityEngine;

public abstract class BaseWorkStation : NetworkBehaviour, IInteractable
{
    #region Field
    [Networked] protected float CurrentWorkAmount { get; set; }
    [Networked] protected float TotalWorkAmount { get; set; }
    [Networked] protected NetworkBool IsCompleted { get; set; }
    [Networked] protected int WorkerCount { get; set; }
    public abstract string InteractDescription { get; }
    protected Creature Worker { get; set; }
    protected Crew CrewWorker => Worker as Crew;
    protected bool _canRememberWork;
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

    public abstract bool TryShowInfoUI(Creature creature, out bool isInteractable);
    protected abstract bool IsInteractable(Creature creature);
    protected abstract void PlayInteractAnimation();
    protected abstract void PlayEffectMusic();
    public virtual bool Interact(Creature creature)
    {
        if (!IsInteractable(creature)) return false;

        Worker = creature;
        Worker.IngameUI.InteractInfoUI.Hide();
        Worker.CreatureState = Define.CreatureState.Interact;
        Worker.CreaturePose = Define.CreaturePose.Stand;
        Worker.IngameUI.WorkProgressBarUI.Show(InteractDescription, CurrentWorkAmount, TotalWorkAmount);

        PlayInteractAnimation();
        Rpc_AddWorker();
        PlayEffectMusic();
        StartCoroutine(ProgressWork());
        return true;
    }

    protected IEnumerator ProgressWork()
    {
        Worker.IngameUI.InteractInfoUI.Show("Cancel interaction");
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
        Managers.SoundMng.Stop();
        Worker.IngameUI.WorkProgressBarUI.Hide();
        Worker.CreatureState = Define.CreatureState.Idle;
        Worker.CreaturePose = Define.CreaturePose.Stand;

        Rpc_RemoveWorker();
    }

    protected virtual void WorkComplete()
    {
        Rpc_WorkComplete();
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

        if (!_canRememberWork && WorkerCount <= 0)
            CurrentWorkAmount = 0f;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void Rpc_UpdateWorkAmount(float deltaTime, float workSpeed)
    {
        CurrentWorkAmount = Mathf.Clamp(CurrentWorkAmount + deltaTime * workSpeed, 0, TotalWorkAmount);
    }

}
