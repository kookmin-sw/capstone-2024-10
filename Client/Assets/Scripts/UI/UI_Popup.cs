/// <summary>
/// 사용자 정의 PopupUI 생성하려면 해당 클래스를 상속하면 된다.
/// </summary>
public class UI_Popup : UI_Base
{
    /// <summary>
    /// 자식에서 Init을 구현하려고 할 때, 해당 Init을 호출해줘야 한다.
    /// </summary>
    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        Managers.UIMng.SetCanvas(gameObject, true);
        return true;
    }

    /// <summary>
    /// 해당 PopupUi를 닫는다.
    /// </summary>
    public virtual void ClosePopupUI()
    {
        Clear();
        Managers.UIMng.ClosePopupUI(this);
    }

    public virtual void Clear()
    {

    }
}
