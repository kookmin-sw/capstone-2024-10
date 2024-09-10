using TMPro;

public class UI_InteractInfo : UI_Base
{
    enum Texts
    {
        Description_text
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TMP_Text>(typeof(Texts));
        return true;
    }

    public void Show(string description)
    {
        GetText(Texts.Description_text).text = $"<sprite=1>  {description}";
        gameObject.SetActive(true);
    }


    public void Hide() => gameObject.SetActive(false);
}

