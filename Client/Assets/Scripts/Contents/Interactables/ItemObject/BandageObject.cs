using Fusion;

public class BandageObject : BaseItemObject
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_SetInfo(NetworkBool canGet)
    {
        DataId = Define.ITEM_BANDAGE_ID;

        base.Rpc_SetInfo(canGet);
    }
}
