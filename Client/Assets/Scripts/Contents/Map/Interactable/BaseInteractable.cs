using Fusion;

public abstract class BaseInteractable : NetworkBehaviour
{
    public virtual string InteractDescription { get; set; }

    public abstract bool IsInteractable(Creature creature);
    public abstract void Interact(Creature creature);
    public abstract void PlayInteractAnimation();
}
