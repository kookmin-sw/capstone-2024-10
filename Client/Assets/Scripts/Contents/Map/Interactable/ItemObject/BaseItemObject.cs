using Fusion;
using UnityEngine;

public abstract class BaseItemObject : NetworkBehaviour, IInteractable
{
    public abstract int DataId { get;}
    public string InteractDescription => $"Get {Managers.ObjectMng.Items[DataId].ItemData.Name}";
    
    public bool TryShowInfoUI(Creature creature, out bool isInteractable)
    {
        isInteractable = false;
        if (creature is not Crew crew) return false;

        if (!crew.Inventory.CheckCanGetItem())
        {
            creature.IngameUI.ErrorTextUI.Show("Inventory is full!");
            return true;
        }

        creature.IngameUI.InteractInfoUI.Show(InteractDescription);
        isInteractable = true;
        return true;
    }

    public bool Interact(Creature creature)
    {
        Crew crew = creature as Crew;

        Rpc_DisableItem();
        Rpc_DespawnItem();

        crew.Inventory.GetItem(DataId);
        return true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_DisableItem()
    {
        gameObject.SetActive(false);
    }
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void Rpc_DespawnItem()
    {
        Runner.Despawn(gameObject.GetComponent<NetworkObject>());
    }


}
