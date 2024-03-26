using Fusion;

public abstract class BaseInteractable : NetworkBehaviour
{
    public abstract void CheckAndInteract(Creature creature);
}
