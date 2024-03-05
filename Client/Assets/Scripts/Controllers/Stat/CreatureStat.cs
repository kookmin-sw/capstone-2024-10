using System;
using Fusion;

public class CreatureStat : NetworkBehaviour
{
    private NetworkString<_16> _name;
    private float _speed;

    public string Name { get => _name.ToString(); set { _name = (NetworkString<_16>)value; StatChangeAction?.Invoke(this); } }
    public float Speed { get => _speed; set { _speed = value; StatChangeAction?.Invoke(this); } }

    public Action<CreatureStat> StatChangeAction;

    public virtual void SetStat(Data.CreatureData creatureData)
    {
        StatChangeAction = null;
        _name = creatureData.Name;
        _speed = creatureData.Speed;
    }

    public virtual float GetStatByDefine(Define.Stat stat)
    {
        switch (stat)
        {
            case Define.Stat.Speed:
                return Speed;
        }

        return -1;
    }
}
