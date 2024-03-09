using UnityEngine;
using Data;

public class Alien : Creature
{
    #region Field

    public AlienData AlienData => CreatureData as AlienData;
    public AlienStat AlienStat => (AlienStat)CreatureStat;

    #endregion

    public override void Rpc_SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Alien;
        Transform.parent = Managers.ObjectMng.AlienRoot;

        base.Rpc_SetInfo(templateID);
    }

    #region Input

    protected override void HandleKeyDown()
    {
        Quaternion cameraRotationY = Quaternion.Euler(0, Camera.transform.rotation.eulerAngles.y, 0);

        Vector3 velocity = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * CreatureStat.WalkSpeed;

        if (velocity == Vector3.zero)
        {
            CreatureState = Define.CreatureState.Idle;
            return;
        }

        Velocity = velocity;
        CreatureState = Define.CreatureState.Move;

        if (Input.GetKey(KeyCode.LeftShift))
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

