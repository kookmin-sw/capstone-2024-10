using Fusion;

public abstract class BaseInteractable : NetworkBehaviour
{
    [Networked] public NetworkString<_16> InteractDescription { get; set; }

    public abstract bool IsInteractable(Creature creature, bool isDoInteract);
    public abstract void Interact(Creature creature);

    public abstract void PlayInteractAnimation();
}
