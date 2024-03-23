using Fusion;

public class Battery : BaseItem
{
    public override bool CheckAndUseItem()
    {
        Rpc_Use();
        return true;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected override void Rpc_Use()
    {

    }
}
