using Data;
using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;

public class Crew : Creature
{
    #region Field
    public CrewData CrewData => CreatureData as CrewData;
    public CrewStat CrewStatus => (CrewStat)CreatureStat;

    private Define.CrewState _crewState;
    [Networked]
    public Define.CrewState CrewState
    {
        get => _crewState;
        set
        {
            if (_crewState != value)
            {
                _crewState = value;
            }
        }
    }
    private Define.AnimState _animState;
    [Networked]
    public Define.AnimState AnimState
    {
        get => _animState;
        set
        {
            if (_animState != value)
            {
                _animState = value;
            }
        }
    }

    //애니메이션을 위한 변수
    private float _SitDown = 0;
    private float _CurrentSpeed = 0;    //현재 속도
    private float _SitWalkSpeed = 0;    //앉아서 걷는 속도
    private float _CurrentHp;  //현재 체력
    private float _CurrentStamina;    //현재 스테미나

    public override void Spawned()
    {
        
        base.Init();
        Rpc_SetInfo(0);
        CrewState = Define.CrewState.Stand;
        Anim.SetFloat("Health", 100);
    }


    public override void Rpc_SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Crew;
        Transform.parent = Managers.ObjectMng.CrewRoot;

        base.Rpc_SetInfo(templateID);

        //CrewStatus.SetStat(CrewData);
        
    }
    #endregion

    void Update()
    {
        HandleKeyDown();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

    }

    #region Input
    protected override void HandleKeyDown()
    {
        base.HandleKeyDown();
        if (Input.GetKey(KeyCode.C))
        {
            if (CrewState == Define.CrewState.Sit)
            {
                CrewState = Define.CrewState.Stand;
            }
            else
            {
                CrewState = Define.CrewState.Sit;
            }
        }

    }
    #endregion

    #region MoveUpdate
    protected override void UpdateIdle()
    {
        switch (CrewState)
        {
            case Define.CrewState.Sit:
                AnimState = Define.AnimState.SitDown;
                UpdateAnimation();
                AnimState = Define.AnimState.SitIdle;
                UpdateAnimation();
                break;
            case Define.CrewState.Stand:
                AnimState = Define.AnimState.Idle;
                UpdateAnimation();
                break;
        }
    }

    protected override void UpdateMove()
    {
        base.UpdateMove();
        KCC.Move(Velocity, 0f);
        
        if (Velocity != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(Velocity);
            KCC.SetLookRotation(newRotation);
        }

        switch (CrewState)
        {
            case Define.CrewState.Sit:
                AnimState = Define.AnimState.SitWalk;
                UpdateAnimation();
                break;

            case Define.CrewState.Stand:
                AnimState = Define.AnimState.Walk;
                UpdateAnimation();
                break;

            case Define.CrewState.Run:

                AnimState = Define.AnimState.Run;
                UpdateAnimation();
                break;
        }

    }

    protected override void UpdateUseItem()
    {
    }

    protected override void UpdateDead()
    {
    }

    #endregion

    #region Event
    public void OnDamaged(int damage)
    {
        CrewStatus.OnDamage(damage);

        if (CrewStatus.Hp <= 0)
        {
            OnDead();
            return;
        }
    }

    public void OnDead()
    {
        CreatureState = Define.CreatureState.Dead;
    }
    #endregion

    #region AnimUpdate
    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();
        switch (AnimState)
        {
            case Define.AnimState.Run:
                PlayAnimationRun();
                break;
            case Define.AnimState.SitDown:
                PlayAnimationSitDown();
                break;
            case Define.AnimState.SitIdle:
                PlayAnimationSitIdle();
                break;
            case Define.AnimState.SitWalk:
                PlayAnimationSitWalk();
                break;
        }
    }
    public override void PlayAnimationIdle()
    {
        float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
        _SitDown = Mathf.Lerp(_SitDown, 0, Runner.DeltaTime * sit_smoothness);
        Anim.SetFloat("Sit", _SitDown);
        float smoothness = 5f; // 조절 가능한 부드러움 계수
        _CurrentSpeed = Mathf.Lerp(_CurrentSpeed, 0, Runner.DeltaTime * smoothness);
        Anim.SetFloat("moveSpeed", _CurrentSpeed);
    }
    public override void PlayAnimationWalk()
    {
        float smoothness = 4f; // 조절 가능한 부드러움 계수
        _CurrentSpeed = Mathf.Lerp(_CurrentSpeed, 2.5f, Runner.DeltaTime * smoothness);
        Anim.SetFloat("moveSpeed", _CurrentSpeed);
    }
    public void PlayAnimationRun()
    {
        float smoothness = 2f; // 조절 가능한 부드러움 계수
        _CurrentSpeed = Mathf.Lerp(_CurrentSpeed, 4, Runner.DeltaTime * smoothness);
        Anim.SetFloat("moveSpeed", _CurrentSpeed);
    }
    public void PlayAnimationSitDown()
    {
        float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
        _SitDown = Mathf.Lerp(_SitDown, 1, Runner.DeltaTime * sit_smoothness);
        Anim.SetFloat("Sit", _SitDown);
    }
    public void PlayAnimationSitIdle()
    {
        float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
        _SitWalkSpeed = Mathf.Lerp(_SitWalkSpeed, 0, Runner.DeltaTime * sit_smoothness);
        Anim.SetFloat("sitSpeed", _SitWalkSpeed);
    }
    public void PlayAnimationSitWalk()
    {
        float sit_smoothness = 5f; // 조절 가능한 부드러움 계수
        _SitWalkSpeed = Mathf.Lerp(_SitWalkSpeed, 1.5f, Runner.DeltaTime * sit_smoothness);
        Anim.SetFloat("sitSpeed", _SitWalkSpeed);
    }
    #endregion
}
