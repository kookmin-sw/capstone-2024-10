using Data;
using Fusion;
using UnityEngine;

public abstract class BaseItem
{
    public int DataId { get; protected set; }
    public ItemData ItemData { get; protected set; }
    public Define.ItemType ItemType { get; set; }

    public Crew Owner { get; protected set; }

    public GameObject ItemGameObject { get; protected set; }

    public virtual void SetInfo(int templateId, Crew owner)
    {
        DataId = templateId;
        ItemData = Managers.DataMng.ItemDataDict[templateId];
        Owner = owner;
    }

    public abstract bool CheckAndUseItem(Crew crew);

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected virtual void Rpc_Use() { }
}
