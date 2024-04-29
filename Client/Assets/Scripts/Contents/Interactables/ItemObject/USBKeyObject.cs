using Fusion;

public class USBKeyObject : BaseItemObject
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_SetInfo(NetworkBool canGet)
    {
        DataId = Define.ITEM_USBKEY_ID;

        base.Rpc_SetInfo(canGet);
    }
}
