public class ItemKitCover : BaseWorkStation
{
    public ItemKit ItemKit { get; set; }

    public override bool IsInteractable(Creature creature)
    {
        return ItemKit.IsInteractable(creature);
    }

    public override bool Interact(Creature creature)
    {
        return ItemKit.Interact(creature);
    }

    protected override void Rpc_WorkComplete() { }

    protected override void Rpc_PlaySound() { }
}
