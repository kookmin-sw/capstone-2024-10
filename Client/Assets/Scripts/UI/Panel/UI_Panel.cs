using System.Collections;
public class UI_Panel : UI_Base
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UIMng.SetCanvas(gameObject, false);
        return true;
    }

    public virtual void ClosePanelUI()
    {
        Destroy(gameObject);
    }
}
