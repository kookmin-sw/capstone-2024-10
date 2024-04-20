using Fusion;

public class BandageObject : BaseItemObject
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_SetInfo()
    {
        DataId = Define.ITEM_Bandage_ID;

        base.Rpc_SetInfo();
    }
}
