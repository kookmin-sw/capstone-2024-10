using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Entry : UI_Base
{
    #region Enums
    public enum Buttons
    {
        Submit,
    }

    public enum Images
    {
    }

    public enum Texts
    {
        Name,
    }

    public enum GameObjects
    {
        NickName,
    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        GetButton((int)Buttons.Submit).onClick.AddListener(SubmitName);
        GetText((int)Texts.Name).text = Managers.NetworkMng.PlayerName;

        return true;
    }

    public void SubmitName()
    {
        var input = GetObject((int)GameObjects.NickName).GetComponent<TMP_InputField>();
        Managers.NetworkMng.PlayerName = input.text;
        input.text = "";
        GetText((int)Texts.Name).text = Managers.NetworkMng.PlayerName;
    }
}
