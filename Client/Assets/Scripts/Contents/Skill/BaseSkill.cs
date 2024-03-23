using Fusion;

public abstract class BaseSkill
{
    public Alien Owner { get; set; }

    public abstract bool CheckAndUseSkill();

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public abstract void Rpc_UseSkill();
}

