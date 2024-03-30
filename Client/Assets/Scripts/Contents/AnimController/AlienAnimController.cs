using UnityEngine;

public class AlienAnimController : BaseAnimController
{
    #region Update

    protected override void PlayIdle()
    {
        XParameter = Mathf.Lerp(XParameter, 0, Runner.DeltaTime * 5);
        ZParameter = Mathf.Lerp(ZParameter, 0, Runner.DeltaTime * 5);
        SpeedParameter = Mathf.Lerp(SpeedParameter, 0, Runner.DeltaTime * 5);

        SetParameterFalse();
        SetFloat("X", XParameter);
        SetFloat("Z", ZParameter);
        SetFloat("Speed", SpeedParameter);
    }

    protected override void PlayMove()
    {
        XParameter = Mathf.Lerp(XParameter, Creature.Direction.x, Runner.DeltaTime * 5);
        ZParameter = Mathf.Lerp(ZParameter, Creature.Direction.z, Runner.DeltaTime * 5);

        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                SetFloat("Z", ZParameter);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 1f, Runner.DeltaTime * 5);
                break;
            case Define.CreaturePose.Sit:
                SetFloat("Z", ZParameter);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 1, Runner.DeltaTime * 5);
                break;
            case Define.CreaturePose.Run:
                SetFloat("Z", ZParameter * 1.8f);
                SpeedParameter = Mathf.Lerp(SpeedParameter, 2, Runner.DeltaTime * 5);
                break;
        }

        SetParameterFalse();
        SetFloat("X", XParameter);
        SetFloat("Speed", SpeedParameter);
    }

    public void PlayBasicAttack()
    {
        SetBool("BasicAttack", true);
    }

    public void PlayCrashDoor()
    {
        SetBool("CrashDoor", true);
    }

    #endregion

    protected override void SetParameterFalse()
    {
        SetBool("BasicAttack", false);
        SetBool("CrashDoor", false);
    }
}
