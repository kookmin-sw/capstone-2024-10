using UnityEngine;
using Fusion;

public class AnimController : NetworkBehaviour
{
    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    public Creature Creature { get; protected set; }
    public Define.CreatureState CreatureState => Creature.CreatureState;
    public Define.CreaturePose CreaturePose => Creature.CreaturePose;

    ////애니메이션을 위한 변수
    public float SitDown { get; protected set; }
    public float StandSpeedParameter { get; protected set; }
    public float SitSpeedParameter { get; protected set; }


    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        NetworkAnim = gameObject.GetComponent<NetworkMecanimAnimator>();
        Creature = gameObject.GetComponent<Crew>();
        SetFloat("Health", 100);

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
        if (Creature.CreatureType == Define.CreatureType.Crew)
        {
            switch (CreaturePose)
            {
                case Define.CreaturePose.Stand:
                    float smoothness = 5f; // 조절 가능한 부드러움 계수
                    SitDown = Mathf.Lerp(SitDown, 0, Runner.DeltaTime * smoothness);
                    SetFloat("Sit", SitDown);
                    StandSpeedParameter = Mathf.Lerp(StandSpeedParameter, 0, Runner.DeltaTime * smoothness);
                    SetFloat("moveSpeed", StandSpeedParameter);
                    break;
                case Define.CreaturePose.Sit:
                    float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
                    SitDown = Mathf.Lerp(SitDown, 1, Runner.DeltaTime * sit_smoothness);
                    SetFloat("Sit", SitDown);
                    SitSpeedParameter = Mathf.Lerp(SitSpeedParameter, 0, Runner.DeltaTime * sit_smoothness);
                    SetFloat("sitSpeed", SitSpeedParameter);
                    break;
            }
        }
        else
        {

        }
        
    }

    protected void PlayMove()
    {
        if (Creature.CreatureType == Define.CreatureType.Crew)
        {
            switch (CreaturePose)
            {
                case Define.CreaturePose.Stand:
                    float stand_smoothness = 4f; // 조절 가능한 부드러움 계수
                    SitDown = Mathf.Lerp(SitDown, 0, Runner.DeltaTime * stand_smoothness);
                    SetFloat("Sit", SitDown);
                    StandSpeedParameter = Mathf.Lerp(StandSpeedParameter, 1.5f, Runner.DeltaTime * stand_smoothness);
                    SetFloat("moveSpeed", StandSpeedParameter);
                    break;
                case Define.CreaturePose.Sit:
                    float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
                    SitDown = Mathf.Lerp(SitDown, 1, Runner.DeltaTime * sit_smoothness);
                    SetFloat("Sit", SitDown);
                    SitSpeedParameter = Mathf.Lerp(SitSpeedParameter, 1, Runner.DeltaTime * sit_smoothness);
                    SetFloat("sitSpeed", SitSpeedParameter);
                    break;
                case Define.CreaturePose.Run:
                    float run_smoothness = 2f; // 조절 가능한 부드러움 계수
                    StandSpeedParameter = Mathf.Lerp(StandSpeedParameter, 2, Runner.DeltaTime * run_smoothness);
                    SetFloat("moveSpeed", StandSpeedParameter);
                    break;
            }
        }
        else
        {

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
