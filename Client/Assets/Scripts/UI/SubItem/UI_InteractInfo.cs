using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InteractInfo : UI_Base
{

    enum Images
    {
        Keycode_bg
    }

    enum Texts
    {
        Keycode_text,
        Description_text
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        return true;
    }

    public void Show(string description)
    {
        GetText(Texts.Description_text).text = description;
        gameObject.SetActive(true);
    }
 

    public void Hide() => gameObject.SetActive(false);
}

