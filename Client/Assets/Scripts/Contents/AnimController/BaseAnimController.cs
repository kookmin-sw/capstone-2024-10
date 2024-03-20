using Fusion;

public abstract class BaseAnimController : NetworkBehaviour
{
    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    public Creature Creature { get; protected set; }
    public Define.CreatureState CreatureState => Creature.CreatureState;
    public Define.CreaturePose CreaturePose => Creature.CreaturePose;

    public float StandSpeedParameter { get; protected set; }

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        NetworkAnim = gameObject.GetComponent<NetworkMecanimAnimator>();
        Creature = gameObject.GetComponent<Crew>();

        SetFloat("moveSpeed", 0);
    }

    #region Update

    public void UpdateAnimation()
    {
        if (HasStateAuthority == false)
            return;

        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                PlayIdle();
                break;
            case Define.CreatureState.Move:
                PlayMove();
                break;
            case Define.CreatureState.Interact:
                PlayInteract();
                break;
            case Define.CreatureState.Dead:
                PlayDead();
                break;
        }
    }

    protected virtual void PlayIdle()
    {
    }

    protected virtual void PlayMove()
    {
    }

    protected virtual void PlayInteract()
    {
    }

    protected virtual void PlayDead()
    {
    }

    #endregion

    #region SetParameter

    protected void SetParameter()
    {

    }

    protected void SetTrigger(string parameter)
    {
        NetworkAnim.SetTrigger("IsRunning");
    }

    protected void SetBool(string parameter, bool value)
    {
        NetworkAnim.Animator.SetBool(parameter, value);
    }

    protected void SetFloat(string parameter, float value)
    {
        NetworkAnim.Animator.SetFloat(parameter, value);
    }

    #endregion
}
