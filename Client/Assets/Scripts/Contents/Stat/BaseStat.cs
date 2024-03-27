using System;
using Fusion;

public abstract class BaseStat : NetworkBehaviour
{
    [Networked] public NetworkString<_16> Name { get; set; }
    [Networked] public float Speed { get; set; }
    [Networked] public float WalkSpeed { get; set; }
    [Networked] public float RunSpeed { get; set; }

    public virtual void SetStat(Data.CreatureData creatureData)
    {
        Name = creatureData.Name;
        Speed = creatureData.WalkSpeed;
        WalkSpeed = creatureData.WalkSpeed;
        RunSpeed = creatureData.RunSpeed;
    }
}
