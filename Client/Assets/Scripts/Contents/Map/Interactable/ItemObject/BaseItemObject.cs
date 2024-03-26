using Fusion;
using UnityEngine;

public abstract class BaseItemObject : BaseInteractable
{
    public Define.ItemType ItemType { get; protected set; }

    public override void Spawned()
    {
        Init();
    }

    protected abstract void Init();

    public override void CheckAndInteract(Creature creature)
    {
        ((Crew)creature).Inventory.CheckAndGetItem(ItemType);
    }
}
