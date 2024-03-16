using ExitGames.Client.Photon.StructWrapping;
using Fusion;

public class BaseItem : NetworkBehaviour, IInteractable
{
    public void Interact(Creature creature)
    {
        creature.Inventory.CheckAndGet(this);
    }
}
