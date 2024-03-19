using System;
using Fusion;
using System.Collections;
using System.Collections.Generic;
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
    private Image _progress;
    private float _totalWorkAmount;

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

    public void SetInfo(string workDescription, float workAmount)
    {
        GetText((int)Texts.WorkDescription).text = workDescription;
        _totalWorkAmount = workAmount;
    }

    private void Update()
    {
        _progress.fillAmount = CurrentWorkAmount / _totalWorkAmount;
    }
}
