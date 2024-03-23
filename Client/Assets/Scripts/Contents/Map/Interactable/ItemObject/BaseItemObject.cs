using Fusion;

public abstract class BaseItemObject : NetworkBehaviour, IInteractable
{
    public Define.ItemType ItemType { get; protected set; }

    public override void Spawned()
    {
        Init();
    }

    protected abstract void Init();

    public void Interact(Creature creature)
    {
        ((Crew)creature).Inventory.CheckAndGetItem(ItemType);
    }
}
