using UnityEngine;
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
                SitParameter = Mathf.Lerp(SitParameter, 0, Runner.DeltaTime * 5);
                break;
            case Define.CreaturePose.Sit:
                SitParameter = Mathf.Lerp(SitParameter, 1, Runner.DeltaTime * 5);
                break;
        }

        XParameter = Mathf.Lerp(XParameter, 0, Runner.DeltaTime * 5);
        ZParameter = Mathf.Lerp(ZParameter, 0, Runner.DeltaTime * 5);
        SpeedParameter = Mathf.Lerp(SpeedParameter, 0, Runner.DeltaTime * 5);

        SetParameterFalse();
        SetFloat("X", XParameter);
        SetFloat("Z", ZParameter);
        SetFloat("SitParameter", SitParameter);
        SetFloat("Speed", SpeedParameter);
    }

    public override void PlayMove()
    {
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
            case Define.CrewActionType.Damaged:
                PlayDamaged();
                break;
            case Define.CrewActionType.Dead:
                PlayDead();
                break;
        }
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

    public void PlayDamaged()
    {
        SetBool("Damaged", true);
    }

    public void PlayDead()
    {
        SetBool("IsDead", true);
    }

    #endregion

    protected override void SetParameterFalse()
    {
        SetBool("KeypadUse", false);
        SetBool("OpenItemKit", false);
        SetBool("OpenDoor", false);
        SetBool("ChargeBattery", false);
        SetBool("Damaged", false);
    }
}
