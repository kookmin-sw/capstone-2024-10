using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_Introduction : UI_Panel
{
    public Action onFinished;
    public CanvasGroup group;

    enum Images
    {
        Background,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));

        return true;
    }

    public void StartFade()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(2.5f);
        sequence.Append(group.DOFade(0.0f, 2.0f).SetEase(Ease.OutQuint));
        sequence.Append(GetImage(Images.Background).DOFade(0.0f, 0.5f).SetEase(Ease.OutQuint));
        sequence.OnComplete(() => onFinished.Invoke());
        sequence.Play();
    }
}
