using Fusion;

public class CrewAnimController : BaseAnimController
{
    [Networked] public float SitParameter { get; set; }

    #region Update

    public override void PlayIdle()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
            case Define.CreaturePose.Run:
                SitParameter = Lerp(SitParameter, 0f, Runner.DeltaTime * 3f);
                break;
            case Define.CreaturePose.Sit:
                SitParameter = Lerp(SitParameter, 1f, Runner.DeltaTime * 3f);
                break;
        }

        XParameter = Lerp(XParameter, 0f, Runner.DeltaTime * 5f);
        ZParameter = Lerp(ZParameter, 0f, Runner.DeltaTime * 5f);
        SpeedParameter = Lerp(SpeedParameter, 0f, Runner.DeltaTime * 5f);

        SetParameterFalse();
        SetFloat("X", XParameter);
        SetFloat("Z", ZParameter);
        SetFloat("SitParameter", SitParameter);
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
                SitParameter = Lerp(SitParameter, 0f, Runner.DeltaTime * 3f);
                SpeedParameter = Lerp(SpeedParameter, 1f, Runner.DeltaTime * 5f);
                break;
            case Define.CreaturePose.Sit:
                SetFloat("Z", ZParameter);
                SitParameter = Lerp(SitParameter, 1f, Runner.DeltaTime * 3f);
                SpeedParameter = Lerp(SpeedParameter, 1f, Runner.DeltaTime * 5f);
                break;
            case Define.CreaturePose.Run:
                SetFloat("Z", ZParameter * 1.8f);
                SitParameter = Lerp(SitParameter, 0f, Runner.DeltaTime * 3f);
                SpeedParameter = Lerp(SpeedParameter, 2f, Runner.DeltaTime * 5f);
                break;
        }

        SetParameterFalse();
        SetFloat("X", XParameter);
        SetFloat("SitParameter", SitParameter);
        SetFloat("Speed", SpeedParameter);
    }

    #endregion

    #region Play

    public void PlayAnim(Define.CrewActionType crewActionType)
    {
        switch (crewActionType)
        {
            case Define.CrewActionType.Damaged:
                PlayDamaged();
                break;
            case Define.CrewActionType.Dead:
                PlayDead();
                break;
            case Define.CrewActionType.KeypadUse:
                PlayKeypadUse();
                break;
            case Define.CrewActionType.OpenItemKit:
                PlayOpenItemKit();
                break;
            case Define.CrewActionType.OpenDoor:
                PlayOpenDoor();
                break;
            case Define.CrewActionType.ChargeBattery:
                PlayChargeBattery();
                break;
            case Define.CrewActionType.Insert:
                PlayInsert();
                break;
            case Define.CrewActionType.FlashBang:
                PlayThrow();
                break;
            case Define.CrewActionType.Bandage:
                PlayBandage();
                break;
        }
    }

    public void PlayDamaged()
    {
        SetBool("Damaged", true);
    }

    public void PlayDead()
    {
        SetBool("IsDead", true);
    }

    public void PlayKeypadUse()
    {
        SetBool("KeypadUse", true);
    }

    public void PlayOpenItemKit()
    {
        SetBool("OpenItemKit", true);
    }

    public void PlayOpenDoor()
    {
        SetBool("OpenDoor", true);
    }

    public void PlayChargeBattery()
    {
        SetBool("ChargeBattery", true);
    }

    public void PlayInsert()
    {
        SetBool("Insert", true);
    }

    public void PlayThrow()
    {
        SetBool("Throw", true);
    }

    public void PlayBandage()
    {
        SetBool("Bandage", true);
    }

    #endregion

    protected override void SetParameterFalse()
    {
        SetBool("KeypadUse", false);
        SetBool("OpenItemKit", false);
        SetBool("OpenDoor", false);
        SetBool("ChargeBattery", false);
        SetBool("Insert", false);
        SetBool("Throw", false);
        SetBool("Damaged", false);
        SetBool("Bandage", false);
    }
}
