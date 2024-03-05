using UnityEngine;

public class AlienStat: CreatureStat
{
    private int _damage;

    public int Damage { get => _damage; set { _damage = value; StatChangeAction?.Invoke(this); } }

    public override void SetStat(Data.CreatureData creatureData)
    {
        base.SetStat(creatureData);

        Data.AlienData alienData = (Data.AlienData)creatureData;

        Damage = alienData.Damage;
    }

    public override float GetStatByDefine(Define.Stat stat)
    {
        switch (stat)
        {
            case Define.Stat.Speed:
                return Speed;
            case Define.Stat.Hp:
                return Damage;
        }

        return -1;
    }
}
