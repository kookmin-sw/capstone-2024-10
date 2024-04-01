public class Battery : BaseItem
{
    public override bool CheckAndUseItem(Crew crew)
    {
        return false;
    }

    protected override void Rpc_Use()
    {

    }
}
