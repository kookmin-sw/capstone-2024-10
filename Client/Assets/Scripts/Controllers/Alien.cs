using UnityEngine;
using Data;

public class Alien : Creature
{
    #region Field

    public AlienData AlienData => CreatureData as AlienData;
    public AlienStat AlienStat => (AlienStat)BaseStat;

    #endregion

    public override void SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Alien;
        Transform.parent = Managers.ObjectMng.AlienRoot;

        base.SetInfo(templateID);
    }

    #region Input

    protected override void HandleInput()
    {
        base.HandleInput();

        if (CreatureState == Define.CreatureState.Interact)
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
    }

    #endregion

    #region Update

    protected override void UpdateIdle()
    {
    }

    protected override void UpdateMove()
    {
        KCC.Move(Velocity, 0f);

        if (Velocity != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(Velocity);
            KCC.SetLookRotation(newRotation);
        }
    }

    protected override void UpdateUse()
    {
    }

    protected override void UpdateDead()
    {
    }

    #endregion

    #region Event

    #endregion
}

