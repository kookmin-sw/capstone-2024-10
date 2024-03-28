using Fusion;

public abstract class BaseInteractable : NetworkBehaviour
{
    public virtual string InteractDescription { get; set; }

    public abstract bool CheckAndInteract(Creature creature);
    
    public abstract void PlayInteract();
}
