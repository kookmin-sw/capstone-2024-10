using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_WorkingBar : UI_Popup
{
    #region UI Setup
    public enum Buttons
    {

    }

    public enum Images
    {
        Background,
        ProgressBar,
    }

    public enum Texts
    {

    }

    public enum GameObjects
    {

    }
    #endregion

    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<Button>(typeof(Buttons));
        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        return true;
    }

    public void CalculateBar(WorkStation myself)
    {        
        //float ratio = GetImage((int)Images.Background).rectTransform.sizeDelta.x / myself.requiredTime;

        //Vector2 size = new Vector2(ratio * myself.workingTime, GetImage((int)Images.Background).rectTransform.sizeDelta.y);
        //Vector2 pos = new Vector2(size.x * 0.5f, 0);

        //GetImage((int)Images.ProgressBar).rectTransform.sizeDelta = size;
        //GetImage((int)Images.ProgressBar).rectTransform.anchoredPosition = pos;

    }
}
