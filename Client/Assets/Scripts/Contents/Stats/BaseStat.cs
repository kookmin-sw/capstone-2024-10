using Data;
using Fusion;

public abstract class BaseStat : NetworkBehaviour
{
    public Creature Creature { get; protected set; }
    public CreatureData CreatureData { get; protected set; }

    [Networked] public NetworkString<_16> Name { get; set; }
    public float Speed { get; set; }
    public float WalkSpeed { get; set; }
    public bool IsUnderErosion { get; set; } = false;

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        Creature = gameObject.GetComponent<Creature>();
    }

    public virtual void SetStat(CreatureData creatureData)
    {
        CreatureData = creatureData;

        Name = creatureData.Name;
        Speed = creatureData.WalkSpeed;
        WalkSpeed = creatureData.WalkSpeed;
    }
}
