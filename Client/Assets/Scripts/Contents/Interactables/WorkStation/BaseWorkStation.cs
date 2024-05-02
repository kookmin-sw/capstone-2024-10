using System.Collections;
using DG.Tweening;
using Fusion;
using UnityEngine;

public abstract class BaseWorkStation : NetworkBehaviour, IInteractable
{
    #region Field
    protected AudioSource AudioSource { get; set; }
    protected Creature Worker { get; set; }
    protected Crew CrewWorker => Worker as Crew;
    protected Define.CrewActionType CrewActionType { get; set; }

    [Networked] protected float CurrentWorkAmount { get; set; } = 0f;
    [Networked] protected float TotalWorkAmount { get; set; }
    protected bool CanRememberWork { get; set; }
    protected string Description { get; set; }

    [Networked] protected int WorkerCount { get; set; }
    [Networked] protected NetworkBool IsCompleted { get; set; } = false;

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

    public virtual bool IsInteractable(Creature creature)
    {
        creature.IngameUI.ErrorTextUI.Hide();
        creature.IngameUI.InteractInfoUI.Hide();

        if (Worker != null) return false;

        if (creature.CreatureState == Define.CreatureState.Interact) return false;

        return true;
    }
    
    public virtual bool Interact(Creature creature)
    {
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
        Worker.IngameUI.InteractInfoUI.Show("Cancel");

        while (CurrentWorkAmount < TotalWorkAmount)
        {
            if (Worker.CreatureState != Define.CreatureState.Interact)
            {
                InterruptWork();
                yield break;
            }

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

        DOVirtual.DelayedCall(0.5f, () => { Worker = null; });
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
