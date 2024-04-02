using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Inventory: NetworkBehaviour
{
    public Crew Owner { get; protected set; }

    public List<int> ItemInventory { get; protected set; }
    [Networked] public int CurrentItemIdx { get; set; }
    public BaseItem CurrentItem => Managers.ObjectMng.Items[ItemInventory[CurrentItemIdx]];

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        Owner = gameObject.GetComponent<Crew>();

        ItemInventory = new List<int>(Define.MAX_ITEM_NUM);
        for (int i = 0; i < Define.MAX_ITEM_NUM; i++)
        {
            ItemInventory.Add(-1);
        }

        CurrentItemIdx = 0;
    }

    public bool CheckCanGetItem()
    {
        for (int i = 0; i < Define.MAX_ITEM_NUM; i++)
        {
            if (ItemInventory[i] == -1)
                return true;
        }

        return false;
    }

    public void GetItem(int itemId)
    {
        if (ItemInventory[CurrentItemIdx] == -1)
        {
            ItemInventory[CurrentItemIdx] = itemId;
            Owner.CrewIngameUI.UI_Inventory.Show(CurrentItemIdx);
            return;
        }

        for (int i = 0; i < Define.MAX_ITEM_NUM; i++)
        {
            if (ItemInventory[i] == -1)
            {
                ItemInventory[i] = itemId;
                Owner.CrewIngameUI.UI_Inventory.Show(i);
                return;
            }
        }
    }

    public bool HasItem(int itemId)
    {
        for (int i = 0; i < Define.MAX_ITEM_NUM; i++)
        {
            if (ItemInventory[i] == itemId) return true;
        }

        return false;
    }

    public void RemoveItem(int itemId)
    {
        if (!HasItem(itemId))
        {
            Debug.LogWarning($"No such item in inventory: {Managers.ObjectMng.Items[itemId]}");
        }
        for (int i = 0; i < Define.MAX_ITEM_NUM; i++)
        {
            if (ItemInventory[i] == itemId)
            {
                ItemInventory[i] = -1;
                break;
            }
        }
    }

    public bool CheckAndUseItem()
    {
        if (ItemInventory[CurrentItemIdx] == -1)
            return false;

        if (!CurrentItem.CheckAndUseItem(Owner))
            return false;

        ItemInventory[CurrentItemIdx] = -1;

        Owner.CrewIngameUI.UI_Inventory.Hide(CurrentItemIdx);
        return true;
    }

    public void DropItem()
    {

    }

    public void ChangeItem(int idx)
    {
        CurrentItemIdx = idx;
    }
}
