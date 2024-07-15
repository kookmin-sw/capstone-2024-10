using System.Text.RegularExpressions;
using TMPro;

public class UI_CurrentSector : UI_Base
{
    enum Texts
    {
        SectorName
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TMP_Text>(typeof(Texts));

        return true;
    }

    public void SetSector(Define.SectorName sector)
    {
        if (GetText(Texts.SectorName) == null)
            return;

        GetText(Texts.SectorName).text =
            sector != Define.SectorName.None ? Util.AddSpaceInText(sector.ToString()) : "";
    }

    public void Hide() => gameObject.SetActive(false);
}
