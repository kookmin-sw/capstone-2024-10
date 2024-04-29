using Data;
using Fusion;
using UnityEngine;

public abstract class BaseItem
{
    public int DataId { get; protected set; }
    public ItemData ItemData { get; protected set; }

    public Crew Owner { get; protected set; }

    public GameObject ItemGameObject { get; protected set; }

    public virtual void SetInfo(int templateId, Crew owner)
    {
        DataId = templateId;
        ItemData = Managers.DataMng.ItemDataDict[templateId];
        Owner = owner;

        string className = Managers.DataMng.ItemDataDict[templateId].Name;
        ItemGameObject = Managers.ResourceMng.Instantiate($"{Define.ITEM_PATH}/{className}", Owner.LeftHand.transform);
    }

    public abstract bool CheckAndUseItem();

    protected virtual void UseItem() { }
}
