using Fusion;
using UnityEngine;

public abstract class BaseItemObject : BaseInteractable
{
    public int DataId { get; protected set; }

    public override void Spawned()
    {
        Init();
    }

    protected abstract void Init();

    public override bool IsInteractable(Creature creature, bool isDoInteract)
    {
        if (creature.CreatureType == Define.CreatureType.Alien)
            return false;

        creature.IngameUI.InteractInfoUI.Show(InteractDescription.ToString());

        if (!(creature.CreatureState == Define.CreatureState.Idle || creature.CreatureState == Define.CreatureState.Move))
            return false;

        if (!((Crew)creature).Inventory.CheckCanGetItem())
            return false;

        if (isDoInteract)
            Interact(creature);

        return true;
    }

    public override void Interact(Creature creature)
    {
        PlayInteractAnimation();
        Rpc_InteractComplete();

        ((Crew)creature).Inventory.GetItem(DataId);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public virtual void Rpc_InteractComplete()
    {
        gameObject.SetActive(false);
    }

    public override void PlayInteractAnimation()
    {

    }
}
