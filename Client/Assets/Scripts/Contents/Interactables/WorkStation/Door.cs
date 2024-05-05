using Fusion;
using UnityEngine;

public class Door : BaseWorkStation
{
    public new string Description => IsOpened ? "Close" : "Open";

    [Networked] public NetworkBool IsOpened { get; set; } = true;

    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    protected override void Init()
    {
        base.Init();

        NetworkAnim = transform.GetComponent<NetworkMecanimAnimator>();
        AudioSource = gameObject.GetComponent<AudioSource>();

        CrewActionType = Define.CrewActionType.OpenDoor;
        CanRememberWork = false;

        TotalWorkAmount = 15f; // only for alien crashing door
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

        if (creature is Alien && IsOpened)
        {
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    public override bool Interact(Creature creature)
    {
        if (WorkerCount > 0)
            return false;

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
        NetworkAnim.Animator.SetBool("OpenParameter", IsOpened);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_AlienWorkComplete()
    {
        gameObject.SetActive(false);
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
}
