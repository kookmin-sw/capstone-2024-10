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
    [Networked] protected bool CanRememberWork { get; set; }
    [Networked] protected int WorkerCount { get; set; }
    [Networked] protected NetworkBool IsCompleted { get; set; } = false;
    [Networked] protected NetworkString<_64> NetworkDescription { get; set; }
    protected string Description
    {
        get => NetworkDescription.ToString();
        set => NetworkDescription = value;
    }

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
        if (creature is Crew) creature.IngameUI.ObjectNameUI.Show(this.GetType().ToString());

        if (Worker != null) return false;

        if (creature.CreatureState == Define.CreatureState.Interact) return false;

        return true;
    }

    public virtual bool Interact(Creature creature)
    {
        Worker = creature;
        Worker.CreatureState = Define.CreatureState.Interact;
        Worker.CreaturePose = Define.CreaturePose.Stand;
        Worker.IngameUI.InteractInfoUI.Hide();
        Worker.IngameUI.ObjectNameUI.Hide();
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

            Rpc_UpdateWorkAmount(Time.deltaTime);
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
        if (AudioSource != null) Rpc_StopSound();

        DOVirtual.DelayedCall(0.3f, () => { Worker = null; });
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
    private void Rpc_UpdateWorkAmount(float deltaTime)
    {
        CurrentWorkAmount = Mathf.Clamp(CurrentWorkAmount + deltaTime, 0, TotalWorkAmount);
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
        WorkerCount = WorkerCount > 0 ? WorkerCount - 1 : 0;

        if (!CanRememberWork && WorkerCount == 0)
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
