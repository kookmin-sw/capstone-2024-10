using DG.Tweening;
using Fusion;
using UnityEngine;

public class Door : BaseWorkStation
{
    private new string Description
    {
        get
        {
            if (Managers.ObjectMng.MyCreature is Alien) return "Destroy Door";
            return IsOpened ? "Close" : "Open";
        }
    }

    [Networked, OnChangedRender(nameof(DisableCollider))] private NetworkBool IsOpened { get; set; } = true;

    private NetworkMecanimAnimator _mecanimAnimator;
    private Collider _collider;
    private OcclusionPortal _occlusionPortal;

    protected override void Init()
    {
        base.Init();

        _mecanimAnimator = transform.GetComponent<NetworkMecanimAnimator>();
        _collider = transform.GetComponent<Collider>();
        _occlusionPortal = transform.parent.GetComponent<OcclusionPortal>();
        _occlusionPortal.open = true;

        CrewActionType = Define.CrewActionType.OpenDoor;
        CanRememberWork = false;

        TotalWorkAmount = 1.5f; // only for alien crashing door
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

        if (creature is Alien && IsOpened)
        {
            return false;
        }

        if (WorkerCount > 0 && Worker == null)
        {
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    public override bool Interact(Creature creature)
    {
        Worker = creature;
        Worker.IngameUI.InteractInfoUI.Hide();
        Worker.CreatureState = Define.CreatureState.Interact;
        Worker.CreaturePose = Define.CreaturePose.Stand;

        Rpc_AddWorker();
        PlayAnim();

        if (creature is Crew)
        {
            WorkComplete();
            InterruptWork();
            Rpc_PlaySound();
        }
        else
        {
            Worker.IngameUI.WorkProgressBarUI.Show(Description, CurrentWorkAmount, TotalWorkAmount);
            Rpc_AlienPlaySound();
            StartCoroutine(ProgressWork());
        }

        return true;
    }

    protected override void WorkComplete()
    {
        switch (Worker)
        {
            case Crew:
                Rpc_WorkComplete();
                break;
            case Alien:
                Rpc_AlienWorkComplete();
                break;
        }
    }

    protected override void PlayAnim()
    {
        if (IsOpened) return;

        switch (Worker)
        {
            case Crew crew:
                crew.CrewAnimController.PlayAnim(CrewActionType);
                break;
            case Alien alien:
                alien.AlienAnimController.PlayAnim(Define.AlienActionType.CrashDoor);
                break;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        IsOpened = !IsOpened;
        _mecanimAnimator.Animator.SetBool("OpenParameter", IsOpened);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_AlienWorkComplete()
    {
        gameObject.SetActive(false);
        _occlusionPortal.open = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        string soundPath = IsOpened ? "Door_Open" : "Door_Close";
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/{soundPath}", 1f, 1f, isLoop: false);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected void Rpc_AlienPlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/Door_Crash", 1f, 1f, isLoop: false);
    }

    private void DisableCollider()
    {
        _collider.enabled = false;
        if (IsOpened) _occlusionPortal.open = true;

        DOVirtual.DelayedCall(0.5f, () =>
        {
            _collider.enabled = true;
            if (!IsOpened) _occlusionPortal.open = false;
        });
    }
}
