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
                Debug.Log("No Idle_Run");
                break;
        }
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
