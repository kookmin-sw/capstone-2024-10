public abstract class BaseItemObject : BaseInteractable
{
    public Define.ItemType ItemType { get; protected set; }

    public override string InteractDescription => $"Get {gameObject.name}";

    public override void Spawned()
    {
        Init();
    }

    protected abstract void Init();

    public override bool CheckAndInteract(Creature creature)
    {
        return ((Crew)creature).Inventory.CheckAndGetItem(ItemType);
    }

    public override void PlayInteract()
    {
        // TODO
    }
}
