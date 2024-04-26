using Fusion;

public class BatteryObject : BaseItemObject
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_SetInfo(NetworkBool canGet)
    {
        DataId = Define.ITEM_BATTERY_ID;

        base.Rpc_SetInfo(canGet);
    }
}
