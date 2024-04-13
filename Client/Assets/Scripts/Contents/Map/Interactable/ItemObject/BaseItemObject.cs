using Fusion;
using UnityEngine;

public abstract class BaseItemObject : NetworkBehaviour, IInteractable
{
    public abstract int DataId { get;}
    public string InteractDescription => $"GET ITEM";

    public bool CheckInteractable(Creature creature)
    {
        creature.IngameUI.ErrorTextUI.Hide();

        if (creature is not Crew crew)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            return false;
        }

        if (creature.CreatureState == Define.CreatureState.Interact)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            return false;
        }

        if (!crew.Inventory.CheckCanGetItem())
        {
            creature.IngameUI.InteractInfoUI.Hide();
            return true;
        }

        creature.IngameUI.ErrorTextUI.Hide();
        creature.IngameUI.InteractInfoUI.Show(InteractDescription);
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
