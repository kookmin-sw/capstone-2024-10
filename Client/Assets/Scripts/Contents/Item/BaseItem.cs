using Data;
using Fusion;

public abstract class BaseItem
{
    public int DataId { get; protected set; }
    public ItemData ItemData { get; protected set; }
    public Define.ItemType ItemType { get; set; }

    public virtual void SetInfo(int templateId)
    {
        DataId = templateId;
        ItemData = Managers.DataMng.ItemDataDict[templateId];
    }

    public abstract bool CheckAndUseItem(Crew crew);

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected abstract void Rpc_Use();
}
