using Fusion;

public class AntipsychoticObject : BaseItemObject
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_SetInfo(NetworkBool canGet)
    {
        DataId = Define.ITEM_ANTIPSYCHOTIC_ID;

        base.Rpc_SetInfo(canGet);
    }
}
