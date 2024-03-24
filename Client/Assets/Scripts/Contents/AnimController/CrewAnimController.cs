using UnityEngine;
using Fusion;

public class CrewAnimController : BaseAnimController
{
    [Networked] public float SitParameter { get; protected set; }

    protected override void Init()
    {
        base.Init();
        CrewStat = gameObject.GetComponent<CrewStat>();

        SetFloat("Action", 0);
        SetFloat("Health", 100);
        SetFloat("Sit", 0);
        SetFloat("sitSpeed", 0);
    }

    #region Update

    protected override void PlayIdle()
    {
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

        SetFloat("X", 0);
        SetFloat("Z", 0);
        SetFloat("SitParameter", SitParameter);
        SpeedParameter = Mathf.Lerp(SpeedParameter, 0, Runner.DeltaTime * 5);
        SetFloat("Speed", SpeedParameter);
    }

    protected override void PlayMove()
    {
        SetFloat("Health", CrewStat.Hp);
        //switch (CreaturePose)
        //{
        //    case Define.CreaturePose.Stand:
        //        CrewSitDownOrUp(0);
        //        CrewStandMove(1.5f);
        //        break;
        //    case Define.CreaturePose.Sit:
        //        CrewSitDownOrUp(1);
        //        CrewSitMove(1);
        //        break;
        //    case Define.CreaturePose.Run:
        //        CrewStandMove(2);
        //        break;
        //}

        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                SetFloat("Z", Creature.Direction.z);
                SitParameter = Mathf.Lerp(SitParameter, 0, Runner.DeltaTime * 5);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 1f, Runner.DeltaTime * 5);
                break;
            case Define.CreaturePose.Sit:
                SetFloat("Z", Creature.Direction.z);
                SitParameter = Mathf.Lerp(SitParameter, 1, Runner.DeltaTime * 5);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 1, Runner.DeltaTime * 5);

                break;
            case Define.CreaturePose.Run:
                SetFloat("Z", Creature.Direction.z * 2);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 2, Runner.DeltaTime * 5);
                break;
        }

        SetFloat("X", Creature.Direction.x);
        SetFloat("SitParameter", SitParameter);
        SetFloat("Speed", SpeedParameter);
    }

    public void PlayInteract()
    {
        // TODO
    }

    public void PlayUseItem()
    {
        // TODO
    }

    public void PlayDead()
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
    private void CrewMoveDirection(float value)
    {
        float smoothness = 5f; // 조절 가능한 부드러움 계수
        MoveDirectionParameter = Mathf.Lerp(MoveDirectionParameter, value, Runner.DeltaTime * smoothness);
        SetFloat("moveDirection", MoveDirectionParameter);
    }
    #endregion

    #region Input
    private void InputHandle()
    {
        VerticalInput = Input.GetAxis("Vertical");
        HorizontalInput = Input.GetAxis("Horizontal");
        if (VerticalInput > 0)
        {
            // 상키를 눌렀을 때 A 애니메이션 재생
            CrewMoveDirection(3);
        }
        else if (VerticalInput < 0)
        {
            // 하키를 눌렀을 때 A 애니메이션 재생
            CrewMoveDirection(0);
        }

        // 좌,우키 입력에 따라 애니메이션 재생
        if (HorizontalInput > 0)
        {
            // 우키를 눌렀을 때 B 애니메이션 재생
            CrewMoveDirection(2);
        }
        else if (HorizontalInput < 0)
        {
            // 좌키를 눌렀을 때 B 애니메이션 재생
            CrewMoveDirection(1);
        }
    }
    #endregion
}
