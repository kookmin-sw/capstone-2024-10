using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class Inventory: NetworkBehaviour
{
    public Crew Owner { get; protected set; }
    public Define.CreatureState CreatureState => Owner.CreatureState;
    public Define.CreaturePose CreaturePose => Owner.CreaturePose;

    public List<BaseItem> Items { get; protected set; }
    [Networked] public int CurrentItemIdx { get; protected set; }
    public BaseItem CurrentItem => Items[CurrentItemIdx];

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        Owner = gameObject.GetComponent<Crew>();

        Items = new List<BaseItem>(Define.MAX_ITEM_NUM);

        CurrentItemIdx = 0;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_CheckAndGetItem(Define.ItemType itemType)
    {
        if (CurrentItem != null)
            return;

        Type type = Type.GetType(itemType.ToString());
        if (type == null)
        {
            Debug.LogError("Failed to Rpc_CheckAndGetItem: " + itemType);
            return;
        }

        BaseItem item = (BaseItem)(Activator.CreateInstance(type));
        item.Owner = Owner;
    }

    protected void UseItem()
    {
        CurrentItem.Rpc_Use();
    }

    protected void DropItem()
    {

    }

    protected void ChangeItem(int idx)
    {
        CurrentItemIdx = idx;
    }
}
