using Fusion;

public class MorphineObject : BaseItemObject
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_SetInfo()
    {
        DataId = Define.ITEM_Morphine_ID;

        base.Rpc_SetInfo();
    }
}
