using Fusion;

public abstract class BaseInteractable : NetworkBehaviour
{
    [Networked] public virtual NetworkString<_16> InteractDescription { get; set; }

    public abstract bool CheckAndInteract(Creature creature);

    public abstract void PlayInteract();
}
