using Fusion;

public abstract class BaseItem
{
    public Crew Owner { get; set; }

    public abstract bool CheckAndUseItem();

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected abstract void Rpc_Use();
}
