using Fusion;
using UnityEngine;

public class AlienAnimController : BaseAnimController
{
    #region Field
    [Networked] bool GetBlind { get; set; }
    [Networked] bool BasicAttack { get; set; }
    [Networked] bool CrashDoor { get; set; }
    [Networked] bool ReadyRoar { get; set; }
    [Networked] bool Roar { get; set; }
    [Networked] bool CursedHowl { get; set; }
    [Networked] bool LeapAttack { get; set; }
    [Networked] bool ReadyLeapAttack {  get; set; }
    [Networked] bool HitDelay { get; set; }
    #endregion

    #region Update

    public override void PlayIdle()
    {
        if (HasStateAuthority)
        {
            XParameter = Lerp(XParameter, 0f, Time.deltaTime * 5f);
            ZParameter = Lerp(ZParameter, 0f, Time.deltaTime * 5f);
            SpeedParameter = Lerp(SpeedParameter, 0f, Time.deltaTime * 5f);

            SetParameterFalse();
            SetFloat("X", XParameter);
            SetFloat("Z", ZParameter);
            SetFloat("Speed", SpeedParameter);
        }
        else
        {
            SetParameterFalse();
            var interpolator = new NetworkBehaviourBufferInterpolator(this);
            SetFloat("Z", interpolator.Float(nameof(ZParameter)));
            SetFloat("X", interpolator.Float(nameof(XParameter)));
            SetFloat("Speed", interpolator.Float(nameof(SpeedParameter)));
        }
    }

    public override void PlayMove()
    {
        if (HasStateAuthority)
        {
            XParameter = Lerp(XParameter, Creature.Direction.x, Time.deltaTime * 5f);
            ZParameter = Lerp(ZParameter, Creature.Direction.z, Time.deltaTime * 5f);

            SetFloat("Z", ZParameter);
            SpeedParameter = Lerp(SpeedParameter, 1f, Time.deltaTime * 5f);

            SetParameterFalse();
            SetFloat("X", XParameter);
            SetFloat("Speed", SpeedParameter);
        }
        else
        {
            SetParameterFalse();
            var interpolator = new NetworkBehaviourBufferInterpolator(this);
            SetFloat("Z", interpolator.Float(nameof(ZParameter)));
            SetFloat("X", interpolator.Float(nameof(XParameter)));
            SetFloat("Speed", interpolator.Float(nameof(SpeedParameter)));
        }
    }

    public override void PlayAction()
    {
        SetBool("GetBlind", GetBlind);
        SetBool("BasicAttack", BasicAttack);
        SetBool("CrashDoor", CrashDoor);
        SetBool("ReadyRoar", ReadyRoar);
        SetBool("Roar", Roar);
        SetBool("CursedHowl", CursedHowl);
        SetBool("LeapAttack", LeapAttack);
        SetBool("ReadyLeapAttack", ReadyLeapAttack);
        SetBool("HitDelay", HitDelay);
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
        GetBlind = true;
    }

    public void PlayCrashDoor()
    {
        CrashDoor = true;
    }

    public void PlayBasicAttack()
    {
        BasicAttack = true;
    }

    public void PlayReadyRoar()
    {
        ReadyRoar = true;
    }

    public void PlayRoar()
    {
        Roar = true;
    }

    public void PlayReadyCursedHowl()
    {
        ReadyRoar = true;
    }

    public void PlayCursedHowl()
    {
        CursedHowl = true;
    }

    public void PlayReadyLeapAttack()
    {
        ReadyLeapAttack = true;
    }

    public void PlayLeapAttack()
    {
        LeapAttack = true;
    }

    public void PlayHitDelay()
    {
        HitDelay = true;
    }

    #endregion

    protected override void SetParameterFalse()
    {
        if (HasStateAuthority)
        {
            GetBlind = false;
            BasicAttack = false;
            CrashDoor = false;
            ReadyRoar = false;
            Roar = false;
            CursedHowl = false;
            LeapAttack = false;
            ReadyLeapAttack = false;
            HitDelay = false;
        }

        SetBool("GetBlind", GetBlind);
        SetBool("BasicAttack", BasicAttack);
        SetBool("CrashDoor", CrashDoor);
        SetBool("ReadyRoar", ReadyRoar);
        SetBool("Roar", Roar);
        SetBool("CursedHowl", CursedHowl);
        SetBool("LeapAttack", LeapAttack);
        SetBool("ReadyLeapAttack", ReadyLeapAttack);
        SetBool("HitDelay", HitDelay);
    }
}
