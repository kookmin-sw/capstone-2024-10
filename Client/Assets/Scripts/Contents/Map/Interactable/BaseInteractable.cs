using Fusion;

public abstract class BaseInteractable : NetworkBehaviour
{
    public abstract bool CheckAndInteract(Creature creature);

    public abstract void PlayInteract();
}
