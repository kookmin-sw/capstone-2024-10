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

    public float CurrentWorkAmount { get; set; }
    private float _totalWorkAmount;
    private Image _progress;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));
        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));

        _progress = GetImage((int)Images.Progress);

        gameObject.SetActive(false);
        return true;
    }

    public void Show(string workDescription, float currentWorkAmount, float totalWorkAmount)
    {
        GetText((int)Texts.WorkDescription).text = workDescription;
        CurrentWorkAmount = currentWorkAmount;
        _totalWorkAmount = totalWorkAmount;
        _progress.fillAmount = CurrentWorkAmount / totalWorkAmount;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        _progress.fillAmount = CurrentWorkAmount / _totalWorkAmount >= 1? 1 : Mathf.Lerp(_progress.fillAmount, CurrentWorkAmount / _totalWorkAmount, 10 * Time.deltaTime);
    }
}
