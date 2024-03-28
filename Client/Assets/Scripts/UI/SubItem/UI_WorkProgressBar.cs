using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_WorkProgressBar : UI_Base
{
    #region UI 목록들
    public enum Images
    {
        Progress
    }

    public enum Texts
    {
        WorkDescription
    }

    public enum GameObjects
    {
    }
    #endregion

    public float TotalWorkAmount { get; protected set; }
    public float CurrentWorkAmount { get; set; }
    public Image Progress { get; protected set; }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        Progress = GetImage((int)Images.Progress);

        gameObject.SetActive(false);
        return true;
    }

    public void Show(string workDescription, float workAmount)
    {
        GetText((int)Texts.WorkDescription).text = workDescription;
        CurrentWorkAmount = 0f;
        TotalWorkAmount = workAmount;
        Progress.fillAmount = 0f;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        CurrentWorkAmount = 0f;
        TotalWorkAmount = 100f;
        Progress.fillAmount = 0f;

        gameObject.SetActive(false);
    }

    private void Update()
    {
        Progress.fillAmount = CurrentWorkAmount / TotalWorkAmount;
    }
}
