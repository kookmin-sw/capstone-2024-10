using Fusion;

public class BatteryObject : BaseItemObject
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_SetInfo()
    {
        DataId = Define.ITEM_Battery_ID;

        base.Rpc_SetInfo();
    }
}
