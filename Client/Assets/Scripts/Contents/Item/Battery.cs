using Fusion;

public class Battery : BaseItem
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_Use()
    {

    }
}
