public class Medicine : BaseItem
{
    public int Value { get; protected set; }

    public override bool CheckAndUseItem(Crew crew)
    {
        if (crew.CrewStat.Hp == crew.CrewStat.MaxHp)
            return false;

        Use(crew);
        return true;
    }

    protected override void Rpc_Use()
    {
    }

    public void Use(Crew crew)
    {
        crew.CrewStat.OnHpChanged(Value);
        crew.Inventory.CurrentItemIdx = -1;
    }
}
