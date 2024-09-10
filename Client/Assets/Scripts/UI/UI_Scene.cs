/// <summary>
/// 사용자 정의 SceneUI 생성하려면 해당 클래스를 상속하면 된다.
/// </summary>
public class UI_Scene : UI_Base
{
    /// <summary>
    /// 자식에서 Init을 구현하려고 할 때, 해당 Init을 호출해줘야 한다.
    /// </summary>
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UIMng.SetCanvas(gameObject, false);
        return true;
    }
}
