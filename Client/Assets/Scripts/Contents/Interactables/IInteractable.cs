using Fusion;

public interface IInteractable
{

    /// <summary>
    /// InfoUI 띄우기를 시도한다. 띄울 UI가 있으면 true, 없으면 false를 반환한다. 동시에 해당 오브젝트와 상호작용 가능한지 여부를 isInteractable로 반환한다.
    /// </summary>
    public bool CheckInteractable(Creature creature);

    public bool Interact(Creature creature);
}
