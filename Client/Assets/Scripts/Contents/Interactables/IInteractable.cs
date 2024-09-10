public interface IInteractable
{
    /// <summary>
    /// 상호작용 정보 UI 또는 에러 UI 디스플레이 및 상호작용 가능 여부 반환
    /// </summary>
    public bool IsInteractable(Creature creature);

    public bool Interact(Creature creature);
}
