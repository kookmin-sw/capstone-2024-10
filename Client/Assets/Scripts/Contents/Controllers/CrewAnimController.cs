using DG.Tweening;
using Fusion;
using System;
using UnityEngine;

public class CrewAnimController : BaseAnimController
{
    #region Field
    [Networked] public float SitParameter { get; set; }
    [Networked] bool KeypadUse { get; set; }
    [Networked] bool OpenItemKit { get; set; }
    [Networked] bool OpenDoor { get; set; }
    [Networked] bool ChargeBattery { get; set; }
    [Networked] bool Insert { get; set; }
    [Networked] bool Throw { get; set; }
    [Networked] bool Damaged { get; set; }
    [Networked] bool Bandage { get; set; }
    [Networked] bool IsDead { get; set; }
    #endregion

    #region Update

    public override void PlayIdle()
    {
        if (HasStateAuthority)
        {
            switch (CreaturePose)
            {
                case Define.CreaturePose.Stand:
                case Define.CreaturePose.Run:
                    SitParameter = Lerp(SitParameter, 0f, Time.deltaTime * 5f);
                    break;
                case Define.CreaturePose.Sit:
                    SitParameter = Lerp(SitParameter, 1f, Time.deltaTime * 5f);
                    break;
            }

            XParameter = Lerp(XParameter, 0f, Time.deltaTime * 5f);
            ZParameter = Lerp(ZParameter, 0f, Time.deltaTime * 5f);
            SpeedParameter = Lerp(SpeedParameter, 0f, Time.deltaTime * 5f);

            SetParameterFalse();
            SetFloat("X", XParameter);
            SetFloat("Z", ZParameter);
            SetFloat("SitParameter", SitParameter);
            SetFloat("Speed", SpeedParameter);
        }
        else
        {
            SetParameterFalse();
            var interpolator = new NetworkBehaviourBufferInterpolator(this);
            SetFloat("Z", interpolator.Float(nameof(ZParameter)));
            SetFloat("X", interpolator.Float(nameof(XParameter)));
            SetFloat("SitParameter", interpolator.Float(nameof(SitParameter)));
            SetFloat("Speed", interpolator.Float(nameof(SpeedParameter)));
        }
    }

    public override void PlayMove()
    {
        if (HasStateAuthority)
        {
            XParameter = Lerp(XParameter, Creature.Direction.x, Time.deltaTime * 5f);
            ZParameter = Lerp(ZParameter, Creature.Direction.z, Time.deltaTime * 5f);

            switch (CreaturePose)
            {
                case Define.CreaturePose.Stand:
                    SetFloat("Z", ZParameter);
                    SitParameter = Lerp(SitParameter, 0f, Time.deltaTime * 5f);
                    SpeedParameter = Lerp(SpeedParameter, 1f, Time.deltaTime * 5f);
                    break;
                case Define.CreaturePose.Sit:
                    SetFloat("Z", ZParameter);
                    SitParameter = Lerp(SitParameter, 1f, Time.deltaTime * 5f);
                    SpeedParameter = Lerp(SpeedParameter, 1f, Time.deltaTime * 5f);
                    break;
                case Define.CreaturePose.Run:
                    SetFloat("Z", ZParameter * 1.8f);
                    SitParameter = Lerp(SitParameter, 0f, Time.deltaTime * 5f);
                    SpeedParameter = Lerp(SpeedParameter, 2f, Time.deltaTime * 5f);
                    break;
            }

            SetParameterFalse();
            SetFloat("X", XParameter);
            SetFloat("SitParameter", SitParameter);
            SetFloat("Speed", SpeedParameter);
        }
        else
        {
            SetParameterFalse();
            var interpolator = new NetworkBehaviourBufferInterpolator(this);
            SetFloat("Z", interpolator.Float(nameof(ZParameter)));
            SetFloat("X", interpolator.Float(nameof(XParameter)));
            SetFloat("SitParameter", interpolator.Float(nameof(SitParameter)));
            SetFloat("Speed", interpolator.Float(nameof(SpeedParameter)));
        }
    }

    public override void PlayAction()
    {
        SetBool("KeypadUse", KeypadUse);
        SetBool("OpenItemKit", OpenItemKit);
        SetBool("OpenDoor", OpenDoor);
        SetBool("ChargeBattery", ChargeBattery);
        SetBool("Insert", Insert);
        SetBool("Throw", Throw);
        SetBool("Damaged", Damaged);
        SetBool("Bandage", Bandage);
        SetBool("IsDead", IsDead);
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
        Damaged = true;
    }

    public void PlayDead()
    {
        IsDead = true;
    }

    public void PlayKeypadUse()
    {
        KeypadUse = true;
    }

    public void PlayOpenItemKit()
    {
        OpenItemKit = true;
    }

    public void PlayOpenDoor()
    {
        OpenDoor = true;
    }

    public void PlayChargeBattery()
    {
        ChargeBattery = true;
    }

    public void PlayInsert()
    {
        Insert = true;
    }

    public void PlayThrow()
    {
        Throw = true;
    }

    public void PlayBandage()
    {
        Bandage = true;
    }

    #endregion

    protected override void SetParameterFalse()
    {
        if (HasStateAuthority)
        {
            KeypadUse = false;
            OpenItemKit = false;
            OpenDoor = false;
            ChargeBattery = false;
            Insert = false;
            Throw = false;
            Damaged = false;
            Bandage = false;
        }

        SetBool("KeypadUse", KeypadUse);
        SetBool("OpenItemKit", OpenItemKit);
        SetBool("OpenDoor", OpenDoor);
        SetBool("ChargeBattery", ChargeBattery);
        SetBool("Insert", Insert);
        SetBool("Throw", Throw);
        SetBool("Damaged", Damaged);
        SetBool("Bandage", Bandage);
    }
}
