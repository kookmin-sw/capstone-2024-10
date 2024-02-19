using Data;
using UnityEngine;

public class Alien : Creature
{
    public AlienData AlienData => CreatureData as AlienData;

    public override void SetInfo(int templateID)
    {
        CreatureType = Define.CreatureType.Alien;
        Transform.parent = Managers.ObjectMng.AlienRoot;

        base.SetInfo(templateID);
    }

    #region Update
    protected override void UpdateIdle()
    {
    }

    protected override void UpdateMove()
    {
    }

    protected override void UpdateSkill()
    {
    }

    protected override void UpdateDead()
    {
    }
    #endregion
}

