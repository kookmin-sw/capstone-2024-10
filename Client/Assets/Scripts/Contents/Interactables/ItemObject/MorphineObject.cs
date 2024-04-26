using Fusion;

public class MorphineObject : BaseItemObject
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_SetInfo(NetworkBool canGet)
    {
        DataId = Define.ITEM_MORPHINE_ID;

        base.Rpc_SetInfo(canGet);
    }
}
