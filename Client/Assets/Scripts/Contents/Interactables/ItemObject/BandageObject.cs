using Fusion;

public class BandageObject : BaseItemObject
{
    public override int DataId => Define.ITEM_BANDAGE_ID;

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_SetInfo(NetworkBool isGettable)
    {
          base.Rpc_SetInfo(isGettable);
    }
}
