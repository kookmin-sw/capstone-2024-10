using System.Collections.Generic;
using Fusion;

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
            return;
        }

        for (int i = 0; i < Define.MAX_ITEM_NUM; i++)
            if (ItemInventory[i] == -1)
            {
                ItemInventory[i] = itemId;
                return;
            }
    }

    public bool CheckAndUseItem(int itemIdx)
    {
        if (ItemInventory[CurrentItemIdx] == -1)
            return false;

        if (!CurrentItem.CheckAndUseItem(Owner))
            return false;

        ItemInventory[CurrentItemIdx] = -1;
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
