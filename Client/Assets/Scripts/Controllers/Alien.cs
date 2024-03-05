using Data;
using UnityEngine;

public class Alien : Creature
{
    #region Field
    public AlienData AlienData => CreatureData as AlienData;
    public AlienStat AlienCreatureStat => (AlienStat)CreatureStat;

    public override void Rpc_SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Alien;
        Transform.parent = Managers.ObjectMng.AlienRoot;

        base.Rpc_SetInfo(templateID);
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

    protected override void UpdateUseSkill()
    {
    }

    protected override void UpdateDead()
    {
    }
    #endregion

    #region Event

    #endregion
}

