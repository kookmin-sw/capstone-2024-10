using System;
using Fusion;

public abstract class CreatureStat : NetworkBehaviour
{
    private NetworkString<_16> _name;
    private float _speed;

    [Networked] public NetworkString<_16> Name { get => _name; set { _name = value; StatChangeAction?.Invoke(this); } }
    [Networked] public float Speed { get => _speed; set { _speed = value; StatChangeAction?.Invoke(this); } }

    public Action<CreatureStat> StatChangeAction;

    public virtual void SetStat(Data.CreatureData creatureData)
    {
        StatChangeAction = null;
        Name = creatureData.Name;
        Speed = creatureData.Speed;
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
