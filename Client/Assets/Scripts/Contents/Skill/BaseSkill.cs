using Fusion;

public class BaseSkill
{
    public Alien Owner { get; set; }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public virtual void Rpc_Use()
    {

    }
}

