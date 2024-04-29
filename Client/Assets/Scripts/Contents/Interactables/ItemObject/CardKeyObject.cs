using Fusion;

public class CardKeyObject : BaseItemObject
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_SetInfo(NetworkBool canGet)
    {
        DataId = Define.ITEM_CARDKEY_ID;

        base.Rpc_SetInfo(canGet);
    }
}
