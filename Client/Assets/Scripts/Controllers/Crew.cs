using UnityEngine;
using Fusion;
using Data;

public class Crew : Creature
{
    #region Field
    public CrewData CrewData => CreatureData as CrewData;
    public CrewStat CrewStat => (CrewStat)CreatureStat;

    
    public float _hAxis { get; set; }
    public float _vAxis { get; set; }
    public float _Speed { get; set; }

    [Networked] public NetworkBool _IsDamaged { get => default; set { } }


    public override void Spawned()
    {
        base.Init();
        Rpc_SetInfo(0);
    }


    public override void Rpc_SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Crew;
        Transform.parent = Managers.ObjectMng.CrewRoot;

        base.Rpc_SetInfo(templateID);

        CrewStat.SetStat(CrewData);

    }
    #endregion

    public void Update()
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
        _hAxis = Input.GetAxisRaw("Horizontal");
        _vAxis = Input.GetAxisRaw("Vertical");

        if (CreatureState == Define.CreatureState.Use)
        {
            // TODO
        }

        if (_hAxis == 0 && _vAxis == 0)
        {
            CreatureState = Define.CreatureState.Idle;
        }
        else
        {
            CreatureState = Define.CreatureState.Move;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (CreaturePose != Define.CreaturePose.Sit)
            {
                CreaturePose = Define.CreaturePose.Run;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (CreaturePose != Define.CreaturePose.Sit)
            {
                CreaturePose = Define.CreaturePose.Stand;
            }
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (CreaturePose != Define.CreaturePose.Sit)
            {
                CreaturePose = Define.CreaturePose.Sit;
            }
            else
            {
                CreaturePose = Define.CreaturePose.Stand;
            }
            
        }
    }

    #endregion

    #region Update

    protected override void UpdateIdle()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                // TODO
                break;
            case Define.CreaturePose.Sit:
                // TODO
                break;
            case Define.CreaturePose.Run:
                Debug.Log("No Idle_Run");
                break;
        }
    }

    protected override void UpdateMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                _Speed = 80;
                break;
            case Define.CreaturePose.Sit:
                _Speed = 35;
                break;
            case Define.CreaturePose.Run:
                _Speed = 100;
                break;
        }

        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);
        Velocity = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * _Speed;

        KCC.Move(Velocity, 0f);

        if (Velocity != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(Velocity);
            KCC.SetLookRotation(newRotation);
        }
    }

    protected override void UpdateUse()
    {
        // TODO
    }

    protected override void UpdateDead()
    {
        // TODO
    }

    #endregion

    #region Event

    public void OnDamaged(int damage)
    {
        CrewStat.OnDamage(damage);

        if (CrewStat.Hp <= 0)
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
}
