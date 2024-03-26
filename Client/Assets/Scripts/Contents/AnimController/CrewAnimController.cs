using UnityEngine;
using Fusion;

public class CrewAnimController : BaseAnimController
{
    [Networked] public float SitParameter { get; protected set; }
    public bool IsAction { get; protected set; }

    public CrewStat CrewStat { get; protected set; }
    public Animator animator { get; protected set; }
    protected override void Init()
    {
        base.Init();
        CrewStat = gameObject.GetComponent<CrewStat>();
        animator = gameObject.GetComponent<Animator>();

        IsAction = false;
    }

    #region Update

    protected override void PlayIdle()
    {
        //SetFloat("Health", CrewStat.Hp);
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
            case Define.CreaturePose.Run:
                SitParameter = Mathf.Lerp(SitParameter, 0, Runner.DeltaTime * 5);
                break;
            case Define.CreaturePose.Sit:
                SitParameter = Mathf.Lerp(SitParameter, 1, Runner.DeltaTime * 5);
                break;
        }

        XParameter = Mathf.Lerp(XParameter, 0, Runner.DeltaTime * 5);
        ZParameter = Mathf.Lerp(ZParameter, 0, Runner.DeltaTime * 5);
        SpeedParameter = Mathf.Lerp(SpeedParameter, 0, Runner.DeltaTime * 5);

        SetBool("KeypadUse", false);
        SetFloat("X", XParameter);
        SetFloat("Z", ZParameter);
        SetFloat("SitParameter", SitParameter);
        SetFloat("Speed", SpeedParameter);
    }

    protected override void PlayMove()
    {
        //SetFloat("Health", CrewStat.Hp);
        XParameter = Mathf.Lerp(XParameter, Creature.Direction.x, Runner.DeltaTime * 5);
        ZParameter = Mathf.Lerp(ZParameter, Creature.Direction.z, Runner.DeltaTime * 5);

        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                SetFloat("Z", ZParameter);
                SitParameter = Mathf.Lerp(SitParameter, 0, Runner.DeltaTime * 5);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 1f, Runner.DeltaTime * 5);
                break;
            case Define.CreaturePose.Sit:
                SetFloat("Z", ZParameter);
                SitParameter = Mathf.Lerp(SitParameter, 1, Runner.DeltaTime * 5);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 1, Runner.DeltaTime * 5);
                break;
            case Define.CreaturePose.Run:
                SetFloat("Z", ZParameter * 1.8f);
                SitParameter = Mathf.Lerp(SitParameter, 0, Runner.DeltaTime * 5);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 2, Runner.DeltaTime * 5);
                break;
        }

        SetBool("KeypadUse", false);
        SetFloat("X", XParameter);
        SetFloat("SitParameter", SitParameter);
        SetFloat("Speed", SpeedParameter);
    }

    public void PlayKeypadUse()
    {
        SetBool("KeypadUse", true);
    }

    public void PlayUseItem()
    {
        if (!IsAction)
        {
            SetTrigger("ButtonPush");
            IsAction = true;
        }
        else
        {
            animator.ResetTrigger("ButtonPush");
        }
    }

    public void PlayDead()
    {
        SetTrigger("IsDead");
    }

    public void PlayReset()
    {
        IsAction = false;
        SetTrigger("Reset");
        animator.ResetTrigger("Reset");
    }

    #endregion

}
