using UnityEngine;
using Fusion;

public class AnimController : NetworkBehaviour
{
    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    public Creature Creature { get; protected set; }
    public Define.CreatureState CreatureState => Creature.CreatureState;
    public Define.CreaturePose CreaturePose => Creature.CreaturePose;

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
            case Define.CreatureState.Use:
                PlayUse();
                break;
            case Define.CreatureState.Dead:
                PlayDead();
                break;
        }
    }

    protected void PlayIdle()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                // TODO
                break;
            case Define.CreaturePose.Sit:
                // TODO
                break;
            case Define.CreaturePose.Run:
                // TODO
                break;
        }
    }

    protected void PlayMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                // TODO
                break;
            case Define.CreaturePose.Sit:
                // TODO
                break;
            case Define.CreaturePose.Run:
                // TODO
                break;
        }
    }

    protected void PlayUse()
    {
        // TODO
    }

    protected void PlayDead()
    {
        // TODO
    }

    #endregion

    #region SetParameter

    protected void SetBoolTrigger(string parameter)
    {
        NetworkAnim.SetTrigger("IsRunning");
    }

    protected void SetBoolTrigger(string parameter, bool value)
    {
        NetworkAnim.Animator.SetBool(parameter, value);
    }

    protected void SetFloatTrigger(string parameter, float value)
    {
        NetworkAnim.Animator.SetFloat(parameter, value);
    }

    #endregion


    #region Legacy
    // public override void PlayAnimationIdle()
    // {
    //     float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
    //     _SitDown = Mathf.Lerp(_SitDown, 0, Runner.DeltaTime * sit_smoothness);
    //     Anim.SetFloat("Sit", _SitDown);
    //     float smoothness = 5f; // 조절 가능한 부드러움 계수
    //     _CurrentSpeed = Mathf.Lerp(_CurrentSpeed, 0, Runner.DeltaTime * smoothness);
    //     Anim.SetFloat("moveSpeed", _CurrentSpeed);
    // }
    // public override void PlayAnimationWalk()
    // {
    //     float smoothness = 4f; // 조절 가능한 부드러움 계수
    //     _CurrentSpeed = Mathf.Lerp(_CurrentSpeed, 2.5f, Runner.DeltaTime * smoothness);
    //     Anim.SetFloat("moveSpeed", _CurrentSpeed);
    // }
    // public void PlayAnimationRun()
    // {
    //     float smoothness = 2f; // 조절 가능한 부드러움 계수
    //     _CurrentSpeed = Mathf.Lerp(_CurrentSpeed, 4, Runner.DeltaTime * smoothness);
    //     Anim.SetFloat("moveSpeed", _CurrentSpeed);
    // }
    // public void PlayAnimationSitDown()
    // {
    //     float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
    //     _SitDown = Mathf.Lerp(_SitDown, 1, Runner.DeltaTime * sit_smoothness);
    //     Anim.SetFloat("Sit", _SitDown);
    // }
    // public void PlayAnimationSitIdle()
    // {
    //     float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
    //     _SitWalkSpeed = Mathf.Lerp(_SitWalkSpeed, 0, Runner.DeltaTime * sit_smoothness);
    //     Anim.SetFloat("sitSpeed", _SitWalkSpeed);
    // }
    // public void PlayAnimationSitWalk()
    // {
    //     float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
    //     _SitWalkSpeed = Mathf.Lerp(_SitWalkSpeed, 1.5f, Runner.DeltaTime * sit_smoothness);
    //     Anim.SetFloat("sitSpeed", _SitWalkSpeed);
    // }
    #endregion
}
