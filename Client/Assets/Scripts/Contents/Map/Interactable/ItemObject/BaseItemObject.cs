public abstract class BaseItemObject : BaseInteractable
{
    public Define.ItemType ItemType { get; protected set; }

    public override string InteractDescription => $"Get {ItemType}";

    public override void Spawned()
    {
        Init();
    }

    protected abstract void Init();

    public override bool IsInteractable(Creature creature)
    {
        return creature is Crew;
    }

    public override void Interact(Creature creature)
    {
        ((Crew)creature).Inventory.CheckAndGetItem(ItemType);
    }

    public override void PlayInteractAnimation()
    {
        // TODO
    }
}
