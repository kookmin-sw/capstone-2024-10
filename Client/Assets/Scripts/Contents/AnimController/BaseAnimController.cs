using Fusion;

public abstract class BaseAnimController : NetworkBehaviour
{
    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    public Creature Creature { get; protected set; }
    public Define.CreatureState CreatureState => Creature.CreatureState;
    public Define.CreaturePose CreaturePose => Creature.CreaturePose;

    public float XParameter { get; protected set; }
    public float ZParameter { get; protected set; }
    public float SpeedParameter { get; protected set; }

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        NetworkAnim = gameObject.GetComponent<NetworkMecanimAnimator>();
        Creature = gameObject.GetComponent<Creature>();
    }

    #region Update

    public void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case Define.CreatureState.Idle:
                PlayIdle();
                break;
            case Define.CreatureState.Move:
                PlayMove();
                break;
        }
    }

    protected virtual void PlayIdle()
    {
    }

    protected virtual void PlayMove()
    {
    }

    #endregion

    #region SetParameter

    protected void SetTrigger(string parameter)
    {
        NetworkAnim.SetTrigger(parameter);
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
