using Data;
using UnityEngine;

public class Alien : Creature
{
    #region Field
    public AlienData AlienData => CreatureData as AlienData;
    public AlienStat AlienCreatureStat => (AlienStat)CreatureStat;
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

        Vector3 velocity = cameraRotationY * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Runner.DeltaTime * CreatureStat.Speed;

        if (Input.GetKey(KeyCode.C))
        {
            // TODO
        }

        if (velocity == Vector3.zero)
        {
            CreatureState = Define.CreatureState.Idle;
            return;
        }

        Velocity = velocity;
        CreatureState = Define.CreatureState.Move;
    }
    #endregion

    #region Update
    protected override void UpdateIdle()
    {
    }

    protected override void UpdateMove()
    {
        KCC.Move(Velocity, 0f);

        Vector3 dir = Velocity;
        dir.y = 0;
        Transform.forward = dir;
    }

    protected override void UpdateDead()
    {
    }
    #endregion

    #region Event

    #endregion
}

