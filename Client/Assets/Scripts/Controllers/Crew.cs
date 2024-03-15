using UnityEngine;
using Fusion;
using Data;

public class Crew : Creature
{
    #region Field

    public CrewData CrewData => CreatureData as CrewData;
    public CrewStat CrewStat => (CrewStat)CreatureStat;

    [Networked] public NetworkBool _IsDamaged { get => default; set { } }

    #endregion
    public override void Spawned()
    {
        base.Init();
    }

    public override void SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Crew;
        Transform.parent = Managers.ObjectMng.CrewRoot;

        base.SetInfo(templateID);

        CrewStat.SetStat(CrewData);

    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

    }

    #region Input

    protected override void HandleInput()
    {
        base.HandleInput();

        if (CreatureState == Define.CreatureState.Use)
        {
            // TODO
            return;
        }

        if (Velocity == Vector3.zero)
        {
            CreatureState = Define.CreatureState.Idle;
            //가만히 서 있는 상태일 때에도 앉기를 하기 위해 return 제거
        }
        else
        {
            CreatureState = Define.CreatureState.Move;  //else를 통해 키 입력이 있는 경우 move로 상태변환
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))    //앉아 있는 상태에서는 달리기가 적용되지 않게 적용
        {
            if (CreaturePose != Define.CreaturePose.Sit)
            {
                CreaturePose = Define.CreaturePose.Run;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))  //앉아 있는 상태에서 shift를 눌렀다 떼어도 자세 변화가 없게 하기 위해
        {
            if (CreaturePose != Define.CreaturePose.Sit)
            {
                CreaturePose = Define.CreaturePose.Stand;
            }
        }
        if (Input.GetKeyDown(KeyCode.C))    //c키를 눌렸을 경우 현재 앉아 있는지, 서있는 상태인지에 따라 반대되는 상태로 변환되도록 설정
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
        //1인칭 카메라 회전에 따라 오브젝트도 회전
        KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);
    }

    protected override void UpdateMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                CreatureStat.Speed = CrewData.WalkSpeed;
                break;
            case Define.CreaturePose.Sit:
                CreatureStat.Speed = CrewData.SitSpeed;
                break;
            case Define.CreaturePose.Run:
                CreatureStat.Speed = CrewData.RunSpeed;
                break;
        }

        KCC.Move(Velocity, 0f);

        //3인칭 오브젝트 회전
        //if (Velocity != Vector3.zero)
        //{
        //    Quaternion newRotation = Quaternion.LookRotation(Velocity);
        //    KCC.SetLookRotation(newRotation);
        //}

        //1인칭 카메라가 회전할 때만 오브젝트 회전
        KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);
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
