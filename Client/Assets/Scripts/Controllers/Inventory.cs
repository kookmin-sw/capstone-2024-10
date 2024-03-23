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

    public void CheckAndGetItem(Define.ItemType itemType)
    {
        if (CurrentItem != null)
            return;

        Rpc_GetItem(itemType);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_GetItem(Define.ItemType itemType)
    {
        Type type = Type.GetType(itemType.ToString());
        if (type == null)
        {
            Debug.LogError("Failed to Rpc_CheckAndGetItem: " + itemType);
            return;
        }

        Items[CurrentItemIdx] = (BaseItem)(Activator.CreateInstance(type));
        Items[CurrentItemIdx].Owner = Owner;
    }

    public void DropItem()
    {

    }

    public void ChangeItem(int idx)
    {
        CurrentItemIdx = idx;
    }
}
