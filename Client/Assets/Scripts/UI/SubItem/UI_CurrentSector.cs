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
        GetText(Texts.SectorName).text = SectorNameEnumToString(sector);
    }

    private static string SectorNameEnumToString(Define.SectorName sector)
    {
        if (sector == Define.SectorName.None) return "";

        string text = sector.ToString();

        // 언더바 제거
        text = text.Replace("_", "");


        // 대문자 앞에 띄어쓰기 추가 (첫 글자 제외)
        text = Regex.Replace(text, "(?<!^)([A-Z])", " $1");

        text = text.Replace("F1", "1F");
        text = text.Replace("F2", "2F");

        return text;
    }
}
