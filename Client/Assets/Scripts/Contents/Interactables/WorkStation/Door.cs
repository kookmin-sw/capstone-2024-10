using Fusion;
using UnityEngine;

public class Door : BaseWorkStation
{
    public new string Description => IsOpened ? "Close" : "Open";

    [Networked] private NetworkBool IsOpened { get; set; } = false;

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

    public override bool CheckInteractable(Creature creature)
    {
        creature.IngameUI.ErrorTextUI.Hide();

        if (creature is Alien && IsOpened)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            return false;
        }

        if (creature.CreatureState == Define.CreatureState.Interact)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    public override bool Interact(Creature creature)
    {
        if (!IsInteractable(creature)) return false;

        Worker = creature;
        Worker.IngameUI.InteractInfoUI.Hide();
        Worker.CreatureState = Define.CreatureState.Interact;
        Worker.CreaturePose = Define.CreaturePose.Stand;

        Rpc_AddWorker();
        PlayAnim();

        if (creature is Crew)
        {
            InterruptWork();
            Rpc_PlaySound();
            WorkComplete();
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
        if (IsOpened)
            AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Interactable/Door_Close");
        else
            AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Interactable/Door_Open");

        AudioSource.volume = 1f;
        AudioSource.loop = false;
        AudioSource.Play();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected void Rpc_AlienPlaySound()
    {
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Interactable/Door_Crash");
        AudioSource.volume = 1f;
        AudioSource.loop = false;
        AudioSource.Play();
    }
}
