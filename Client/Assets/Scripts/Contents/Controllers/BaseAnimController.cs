using Fusion;
using UnityEngine;

public abstract class BaseAnimController : NetworkBehaviour
{
    public Animator Animator { get; protected set; }
    public Creature Creature { get; protected set; }
    public Define.CreatureState CreatureState => Creature.CreatureState;
    [Networked] public Define.CreaturePose CreaturePose { get; set; }

    [Networked] public float XParameter { get; set; }
    [Networked] public float ZParameter { get; set; }
    [Networked] public float SpeedParameter { get; set; }

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        Creature = gameObject.GetComponent<Creature>();
        Animator = gameObject.GetComponent<Animator>();
    }

    #region Update
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            CreaturePose = Creature.CreaturePose;
        }
    }

    public abstract void PlayIdle();

    public abstract void PlayMove();

    public abstract void PlayAction();

    #endregion

    #region SetParameter

    protected void SetTrigger(string parameter)
    {
        Animator.SetTrigger(parameter);
    }

    protected void SetBool(string parameter, bool value)
    {
        Animator.SetBool(parameter, value);
    }

    protected void SetFloat(string parameter, float value)
    {
        Animator.SetFloat(parameter, value);
    }

    protected float Lerp(float start, float end, float time)
    {
        float value = Mathf.Lerp(start, end, time);
        if (value > end - 0.01f && value < end + 0.01f)
            value = end;

        return value;
    }

    protected abstract void SetParameterFalse();

    #endregion
}
