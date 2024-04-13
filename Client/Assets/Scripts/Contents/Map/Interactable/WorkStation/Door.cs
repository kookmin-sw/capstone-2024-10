using Fusion;

public class Door : BaseWorkStation
{
    [Networked] private NetworkBool IsOpened { get; set; }
    public override string InteractDescription => IsOpened ? "CLOSE DOOR" : "OPEN DOOR";

    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    protected override void Init()
    {
        base.Init();

        NetworkAnim = transform.GetComponent<NetworkMecanimAnimator>();

        IsOpened = false;
        CanRememberWork = false;

        TotalWorkAmount = 5f; // only for alien crashing door
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
            PlayEffectMusic();
        }
        else
        {
            Worker.IngameUI.WorkProgressBarUI.Show(InteractDescription, CurrentWorkAmount, TotalWorkAmount);
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

    protected override void PlayEffectMusic()
    {
        if (IsOpened)
        {
            Managers.SoundMng.Play("Music/Clicks/Door_open", Define.SoundType.Effect, 1f);
        }
        else
        {
            Managers.SoundMng.Play("Music/Clicks/Door_close", Define.SoundType.Effect, 0.8f);
        }
    }
}
