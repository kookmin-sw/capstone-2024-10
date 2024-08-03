using Data;
using Fusion;

public abstract class BaseItemObject : NetworkBehaviour, IInteractable
{
    public abstract int DataId { get; }
    public ItemData ItemData => Managers.DataMng.ItemDataDict[DataId];
    public string Description => $"Take {ItemData.Name}";

    [Networked] public NetworkBool IsGettable { get; set; } = true;

    public override void Spawned()
    {
        Init();
    }

    public virtual void Init() { }

    public virtual void SetInfo(NetworkBool isGettable)
    {
        IsGettable = isGettable;
    }

    public bool IsInteractable(Creature creature)
    {
        if (!IsGettable)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Hide();
            return false;
        }

        if (creature is not Crew crew)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Hide();
            return false;
        }

        if (crew.CreatureState == Define.CreatureState.Interact)
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Hide();
            return false;
        }

        if (crew.Inventory.IsFull())
        {
            creature.IngameUI.InteractInfoUI.Hide();
            creature.IngameUI.ErrorTextUI.Show("Inventory is full!");
            return false;
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
