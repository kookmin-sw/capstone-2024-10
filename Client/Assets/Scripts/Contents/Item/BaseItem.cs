using Fusion;

public class BaseItem
{
    public Creature Owner { get; set; }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public virtual void Rpc_Use()
    {

    }
}
