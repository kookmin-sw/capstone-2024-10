using TMPro;

public class UI_ObjectName : UI_Base
{
    enum Texts
    {
        Name
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TMP_Text>(typeof(Texts));
        gameObject.SetActive(false);
        return true;
    }

    public void Show(string objectName)
    {
        GetText(Texts.Name).text = Util.AddSpaceInText(objectName);
        gameObject.SetActive(true);
    }


    public void Hide() => gameObject.SetActive(false);
}
