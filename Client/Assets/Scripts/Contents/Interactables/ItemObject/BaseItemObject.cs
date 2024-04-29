using Data;
using Fusion;

public abstract class BaseItemObject : NetworkBehaviour, IInteractable
{
    public int DataId { get; protected set; }
    public ItemData ItemData { get; protected set; }
    public string Description { get; protected set; }

    [Networked] public NetworkBool CanGet { get; set; }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public virtual void Rpc_SetInfo(NetworkBool canGet)
    {
        ItemData = Managers.DataMng.ItemDataDict[DataId];

        Description = $"Take {ItemData?.Name}";

        CanGet = canGet;
    }

    public bool CheckInteractable(Creature creature)
    {
        creature.IngameUI.ErrorTextUI.Hide();

        if (!CanGet)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            return false;
        }

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
        creature.IngameUI.InteractInfoUI.Show(Description);
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
