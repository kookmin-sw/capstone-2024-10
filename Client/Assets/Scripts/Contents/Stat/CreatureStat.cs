using System;
using Fusion;

public abstract class CreatureStat : NetworkBehaviour
{
    [Networked] public NetworkString<_16> Name { get => default; set { } }
    [Networked] public float Speed { get; set; }
    [Networked] public float WalkSpeed { get; set; }
    [Networked] public float RunSpeed { get; set; }

    public virtual void SetStat(Data.CreatureData creatureData)
    {
        Name = creatureData.Name;
        Speed = creatureData.WalkSpeed;
        WalkSpeed = creatureData.WalkSpeed;
        WalkSpeed = creatureData.RunSpeed;
    }
}
