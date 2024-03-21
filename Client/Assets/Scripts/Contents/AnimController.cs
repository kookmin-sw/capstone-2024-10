using UnityEngine;
using Fusion;

public class AnimController : NetworkBehaviour
{
    public NetworkMecanimAnimator NetworkAnim { get; protected set; }

    public Creature Creature { get; protected set; }
    public Define.CreatureState CreatureState => Creature.CreatureState;
    public Define.CreaturePose CreaturePose => Creature.CreaturePose;
    public Crew Crew { get; protected set; }

    ////Crew 애니메이션을 위한 변수
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
        Creature = gameObject.GetComponent<Creature>();

        Crew = gameObject.GetComponent<Crew>();
        SetFloat("Health", 100);
        SetFloat("Sit", 0);
        SetFloat("moveSpeed", 0);
        SetFloat("sitSpeed", 0);
    }

    #region Update
    public void UpdateAnimation()
    {
        if (HasStateAuthority == false)
            return;
        SetFloat("Health", Crew.CrewStat.Hp);
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

    protected void PlayIdle()
    {
        if (Creature.CreatureType == Define.CreatureType.Crew)
        {
            switch (CreaturePose)
            {
                case Define.CreaturePose.Stand:
                case Define.CreaturePose.Run:
                    CrewSitDownOrUp(0);
                    CrewStandMove(0);
                    break;
                case Define.CreaturePose.Sit:
                    CrewSitDownOrUp(1);
                    CrewSitMove(0);
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
                    CrewSitDownOrUp(0);
                    CrewStandMove(1.5f);
                    break;
                case Define.CreaturePose.Sit:
                    CrewSitDownOrUp(1);
                    CrewSitMove(1);
                    break;
                case Define.CreaturePose.Run:
                    CrewStandMove(2);
                    break;
            }
        }
        else
        {

        }

    }

    protected void PlayInteract()
    {
        // TODO
    }

    protected void PlayDead()
    {
        // TODO
    }

    #endregion

    #region CrewAnim
    private void CrewSitDownOrUp(float value)
    {
        float smoothness = 5f; // 조절 가능한 부드러움 계수
        SitDown = Mathf.Lerp(SitDown, value, Runner.DeltaTime * smoothness);
        SetFloat("Sit", SitDown);
    }
    private void CrewSitMove(float value)
    {
        float smoothness = 5f; // 조절 가능한 부드러움 계수
        SitSpeedParameter = Mathf.Lerp(SitSpeedParameter, value, Runner.DeltaTime * smoothness);
        SetFloat("sitSpeed", SitSpeedParameter);
    }
    private void CrewStandMove(float value)
    {
        float smoothness = 5f; // 조절 가능한 부드러움 계수
        StandSpeedParameter = Mathf.Lerp(StandSpeedParameter, value, Runner.DeltaTime * smoothness);
        SetFloat("moveSpeed", StandSpeedParameter);
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
