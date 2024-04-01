public abstract class BaseItemObject : BaseInteractable
{
    public int DataId { get; protected set; }

    public override void Spawned()
    {
        Init();
    }

    protected abstract void Init();

    public override bool CheckAndInteract(Creature creature)
    {
        if (creature.CreatureType == Define.CreatureType.Alien)
            return false;

        if (!((Crew)creature).Inventory.CheckAndGetItem(DataId))
            return false;

        gameObject.SetActive(false);
        return true;
    }

    public override void PlayInteract()
    {
        // TODO
    }
}
