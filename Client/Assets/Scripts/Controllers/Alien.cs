using UnityEngine;
using Data;

public class Alien : Creature
{
    #region Field

    public AlienData AlienData => CreatureData as AlienData;
    public AlienStat AlienStat => (AlienStat)BaseStat;

    #endregion
    public override void Spawned()
    {
        base.Init();
    }

    public override void SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Alien;
        Transform.parent = Managers.ObjectMng.AlienRoot;

        base.SetInfo(templateID);

        AlienStat.SetStat(AlienData);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
    }

    #region Input

    protected override void HandleInput()
    {
        base.HandleInput();

        if (CreatureState == Define.CreatureState.Interact)
        {
            if (Velocity == Vector3.zero)
            {
                CreatureState = Define.CreatureState.Idle;
            }
            else
            {
                CreatureState = Define.CreatureState.Move;
            }
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
        if (Input.GetMouseButtonDown(0))
        {
            CreatureState = Define.CreatureState.Interact;
        }
    }

    #endregion

    #region Update

    protected override void UpdateIdle()
    {
        if (IsFirstPersonView)
        {
            KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);
        }
    }

    protected override void UpdateMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                BaseStat.Speed = AlienData.WalkSpeed;
                break;
            case Define.CreaturePose.Run:
                BaseStat.Speed = AlienData.RunSpeed;
                break;
        }
        if (IsFirstPersonView)
        {
            KCC.SetLookRotation(0, CreatureCamera.transform.rotation.eulerAngles.y);
        }
        if (Velocity != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(Velocity);
            KCC.SetLookRotation(newRotation);
        }

        KCC.Move(Velocity, 0f);

    }

    protected override void UpdateUse()
    {
        //AlienSkill alienSkill = new AlienSkill();
        //alienSkill.Rpc_Use();
    }

    protected override void UpdateDead()
    {
    }

    #endregion

    #region Event

    #endregion
}

