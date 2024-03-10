using UnityEngine;
using Fusion;
using Data;

public class Crew : Creature
{
    #region Field
    public CrewData CrewData => CreatureData as CrewData;
    public CrewStat CrewStat => (CrewStat)CreatureStat;

    [Networked] public NetworkBool _IsDamaged { get => default; set { } }

    //애니메이션을 위한 변수
    private float _SitDown = 0;
    private float _CurrentSpeed = 0;    //현재 속도
    private float _SitWalkSpeed = 0;    //앉아서 걷는 속도
    private float _CurrentHp;  //현재 체력
    private float _CurrentStamina;    //현재 스테미나

    public override void Spawned()
    {

        base.Init();
    }


    public override void Rpc_SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Crew;
        Transform.parent = Managers.ObjectMng.CrewRoot;

        base.Rpc_SetInfo(templateID);

        CrewStat.SetStat(CrewData);

    }
    #endregion

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

    }

    #region Input

    protected override void HandleKeyDown()
    {
        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);

        Velocity = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * CreatureStat.WalkSpeed;

        if (CreatureState == Define.CreatureState.Use)
        {
            // TODO
            return;
        }

        if (Velocity == Vector3.zero)
        {
            CreatureState = Define.CreatureState.Idle;
            return;
        }

        CreatureState = Define.CreatureState.Move;

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            CreaturePose = Define.CreaturePose.Run;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            CreaturePose = Define.CreaturePose.Stand;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            CreaturePose = Define.CreaturePose.Sit;
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
                // TODO
                break;
        }
    }

    protected override void UpdateMove()
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
