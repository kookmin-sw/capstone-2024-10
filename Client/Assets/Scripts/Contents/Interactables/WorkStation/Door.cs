using Fusion;
using UnityEngine;

public class Door : BaseWorkStation
{
    [Networked] private NetworkBool IsOpened { get; set; }
    public override string InteractDescription => IsOpened ? "Close" : "Open";

    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    public AudioSource AudioSource { get; protected set; }
    protected override void Init()
    {
        base.Init();

        NetworkAnim = transform.GetComponent<NetworkMecanimAnimator>();
        AudioSource = gameObject.GetComponent<AudioSource>();

        IsOpened = false;
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

        creature.IngameUI.InteractInfoUI.Show(InteractDescription);
        return true;
    }

    public override bool Interact(Creature creature)
    {
        if (!IsInteractable(creature)) return false;

        Worker = creature;
        Worker.IngameUI.InteractInfoUI.Hide();
        Worker.CreatureState = Define.CreatureState.Interact;
        Worker.CreaturePose = Define.CreaturePose.Stand;

        PlayInteractAnimation();
        Rpc_AddWorker();

        if (creature is Crew)
        {
            InterruptWork();
            WorkComplete();
        }
        else
        {
            Worker.IngameUI.WorkProgressBarUI.Show(InteractDescription, CurrentWorkAmount, TotalWorkAmount);
            StartCoroutine(ProgressWork());
        }
        Rpc_PlayEffectMusic(Worker);
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

    protected override void PlayInteractAnimation()
    {
        if (IsOpened) return;

        switch (Worker)
        {
            case Crew crew:
                crew.CrewAnimController.PlayOpenDoor();
                break;
            case Alien alien:
                alien.AlienAnimController.PlayCrashDoor();
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
        //Managers.NetworkMng.Runner.Despawn(gameObject.GetComponent<NetworkObject>());
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlayEffectMusic(Creature creature)
    {
        if (creature is Crew)
        {
            if (IsOpened)
            {
                AudioSource.volume = 1f;
                AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip("Music/Clicks/Door_open");
                AudioSource.Play();
            }
            else
            {
                AudioSource.volume = 1f;
                AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip("Music/Clicks/Door_close");
                AudioSource.Play();
            }
        }
        else
        {
            AudioSource.volume = 1f;
            AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip("Music/Clicks/Monster_attack1");
            AudioSource.Play();
        }

    }
}
