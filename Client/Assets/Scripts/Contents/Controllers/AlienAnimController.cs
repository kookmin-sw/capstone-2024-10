public class AlienAnimController : BaseAnimController
{
    #region Update

    public override void PlayIdle()
    {
        XParameter = Lerp(XParameter, 0f, Runner.DeltaTime * 5f);
        ZParameter = Lerp(ZParameter, 0f, Runner.DeltaTime * 5f);
        SpeedParameter = Lerp(SpeedParameter, 0f, Runner.DeltaTime * 5f);

        SetParameterFalse();
        SetFloat("X", XParameter);
        SetFloat("Z", ZParameter);
        SetFloat("Speed", SpeedParameter);
    }

    public override void PlayMove()
    {
        XParameter = Lerp(XParameter, Creature.Direction.x, Runner.DeltaTime * 5f);
        ZParameter = Lerp(ZParameter, Creature.Direction.z, Runner.DeltaTime * 5f);

        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                SetFloat("Z", ZParameter);
                SpeedParameter = Lerp(SpeedParameter, 1f, Runner.DeltaTime * 5f);
                break;
            case Define.CreaturePose.Sit:
                SetFloat("Z", ZParameter);
                SpeedParameter = Lerp(SpeedParameter, 1f, Runner.DeltaTime * 5f);
                break;
            case Define.CreaturePose.Run:
                SetFloat("Z", ZParameter * 1.8f);
                SpeedParameter = Lerp(SpeedParameter, 2f, Runner.DeltaTime * 5f);
                break;
        }

        SetParameterFalse();
        SetFloat("X", XParameter);
        SetFloat("Speed", SpeedParameter);
    }

    #endregion

    #region Play

    public void PlayAnim(Define.AlienActionType alienActionType)
    {
        switch (alienActionType)
        {
            case Define.AlienActionType.GetBlind:
                PlayGetBlind();
                break;
            case Define.AlienActionType.CrashDoor:
                PlayCrashDoor();
                break;
            case Define.AlienActionType.BasicAttack:
                PlayBasicAttack();
                break;
            case Define.AlienActionType.ReadyRoar:
                PlayReadyRoar();
                break;
            case Define.AlienActionType.Roar:
                PlayRoar();
                break;
            case Define.AlienActionType.ReadyCursedHowl:
                PlayReadyCursedHowl();
                break;
            case Define.AlienActionType.CursedHowl:
                PlayCursedHowl();
                break;
            case Define.AlienActionType.ReadyLeapAttack:
                PlayReadyLeapAttack();
                break;
            case Define.AlienActionType.LeapAttack:
                PlayLeapAttack();
                break;
            case Define.AlienActionType.HitDelay:
                PlayHitDelay();
                break;
        }
    }

    public void PlayGetBlind()
    {
        SetBool("GetBlind", true);
    }

    public void PlayCrashDoor()
    {
        SetBool("CrashDoor", true);
    }

    public void PlayBasicAttack()
    {
        SetBool("BasicAttack", true);
    }

    public void PlayReadyRoar()
    {
        SetBool("ReadyRoar", true);
    }

    public void PlayRoar()
    {
        SetBool("Roar", true);
    }

    public void PlayReadyCursedHowl()
    {
        SetBool("ReadyRoar", true);
    }

    public void PlayCursedHowl()
    {
        SetBool("CursedHowl", true);
    }

    public void PlayReadyLeapAttack()
    {
        SetBool("ReadyLeapAttack", true);
    }

    public void PlayLeapAttack()
    {
        SetBool("LeapAttack", true);
    }

    public void PlayHitDelay()
    {
        SetBool("HitDelay", true);
    }

    #endregion

    protected override void SetParameterFalse()
    {
        SetBool("GetBlind", false);
        SetBool("BasicAttack", false);
        SetBool("CrashDoor", false);
        SetBool("ReadyRoar", false);
        SetBool("Roar", false);
        SetBool("CursedHowl", false);
        SetBool("LeapAttack", false);
        SetBool("ReadyRoar", false);
        SetBool("ReadyLeapAttack", false);
        SetBool("HitDelay", false);
    }
}
