using Fusion;

public class BatteryObject : BaseItemObject
{
    public override int DataId => Define.ITEM_BATTERY_ID;

    //[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    //public override void Rpc_SetInfo(NetworkBool isGettable)
    //{
    //    //DataId = Define.ITEM_BATTERY_ID;

    //    base.Rpc_SetInfo(isGettable);
    //}
}
