using System.Collections.Generic;
using Fusion;

public class Inventory: NetworkBehaviour
{
    public Crew Crew { get; protected set; }
    public Define.CreatureState CreatureState => Crew.CreatureState;
    public Define.CreaturePose CreaturePose => Crew.CreaturePose;

    public List<BaseItem> Items;

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        Crew = gameObject.GetComponent<Crew>();

        Items = new List<BaseItem>();
    }

    public void CheckAndGet(NetworkBehaviour item)
    {

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_Get(NetworkBehaviour item)
    {

    }

    protected void Use()
    {

    }
}
