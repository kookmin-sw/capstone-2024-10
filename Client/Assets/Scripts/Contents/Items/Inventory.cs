using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fusion;
using UnityEngine;

public class Inventory: NetworkBehaviour
{
    public Crew Owner { get; protected set; }

    public Dictionary<int, BaseItem> ItemDict { get; protected set; }
    public List<int> ItemInventory { get; protected set; }

    public int CurrentItemIdx { get; set; }
    public BaseItem CurrentItem => ItemDict[ItemInventory[CurrentItemIdx]];

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        Owner = gameObject.GetComponent<Crew>();

        ItemDict = new Dictionary<int, BaseItem>
        {
            [-1] = null
        };

        foreach (var itemData in Managers.DataMng.ItemDataDict)
        {
            Type itemType = Type.GetType(itemData.Value.Name);
            if (itemType == null)
            {
                Debug.LogError("Failed to BindAction: " + itemData.Value.Name);
                return;
            }

            BaseItem item = (BaseItem)(Activator.CreateInstance(itemType));
            item.SetInfo(itemData.Key, Owner);

            ItemDict[itemData.Key] = item;
        }
    }

    public void SetInfo()
    {
        ItemInventory = new List<int>(Define.MAX_ITEM_NUM);
        for (int i = 0; i < Define.MAX_ITEM_NUM; i++)
        {
            ItemInventory.Add(-1);
        }

        CurrentItemIdx = 0;
    }

    public bool IsFull()
    {
        for (int i = 0; i < Define.MAX_ITEM_NUM; i++)
        {
            if (ItemInventory[i] == -1)
                return false;
        }
        return true;
    }

    public void GetItem(int itemId)
    {
        if (ItemInventory[CurrentItemIdx] == -1)
        {
            ItemInventory[CurrentItemIdx] = itemId;
            Owner.CrewIngameUI.UI_Inventory.Show(CurrentItemIdx, itemId);
            Rpc_ShowItem(itemId);
            return;
        }

        for (int i = 0; i < Define.MAX_ITEM_NUM; i++)
        {
            if (ItemInventory[i] == -1)
            {
                ItemInventory[i] = itemId;
                Owner.CrewIngameUI.UI_Inventory.Show(i, itemId);
                return;
            }
        }
    }

    public int RemoveItem()
    {
        if (ItemInventory[CurrentItemIdx] == -1)
            return -1;

        Rpc_HideItem(ItemInventory[CurrentItemIdx]);
        Owner.CrewIngameUI.UI_Inventory.Hide(CurrentItemIdx);

        int itemId = CurrentItem.DataId;
        ItemInventory[CurrentItemIdx] = -1;

        return itemId;
    }

    public bool CheckAndUseItem()
    {
        if (ItemInventory[CurrentItemIdx] == -1)
            return false;

        if (!CurrentItem.CheckAndUseItem())
            return false;

        RemoveItem();
        return true;
    }

    public bool DropItem()
    {
        if (ItemInventory[CurrentItemIdx] == -1)
            return false;

        Ray ray = new Ray(Owner.Head.transform.position, Owner.Head.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 1f, layerMask: LayerMask.GetMask("MapObject", "WorkStation")))
        {
            Debug.DrawRay(ray.origin, ray.direction * 1f, Color.blue, 0.5f);
            return false;
        }

        NetworkObject no = Managers.ObjectMng.SpawnItemObject(RemoveItem(), Owner.Head.transform.position + Owner.Head.transform.forward, true);
        //no.transform.SetParent(gameObject.transform);

        return true;
    }

    public void ChangeItem(int idx)
    {
        if (CurrentItem != null)
            Rpc_HideItem(ItemInventory[CurrentItemIdx]);

        CurrentItemIdx = idx;

        if (CurrentItem != null)
            Rpc_ShowItem(ItemInventory[idx]);

        Owner.CrewIngameUI.UI_Inventory.Highlight(idx);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_ShowItem(int itemId)
    {
        ItemDict[itemId].ItemGameObject.SetActive(true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_HideItem(int itemId)
    {
        ItemDict[itemId].ItemGameObject.SetActive(false);
    }
}
