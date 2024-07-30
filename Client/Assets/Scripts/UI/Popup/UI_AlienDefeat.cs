using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AlienDefeat : UI_Panel
{
    enum Buttons
    {
        Quit,
    }

    enum Texts
    {
        Text1,
        Text2,
        Text3
    }

    private CanvasGroup CanvasGroup;
    private Coroutine FadeCor;
    private float accumTime = 0f;
    private bool isAnimating = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));

        GetButton((int)Buttons.Quit).onClick.AddListener(ExitGame);
        GetText(Texts.Text3).alpha = 0f;

        CanvasGroup = GetComponent<CanvasGroup>();
        StartFadeIn();
        return true;
    }

    public void StartFadeIn()
    {
        if (FadeCor != null)
        {
            StopAllCoroutines();
            FadeCor = null;
        }
        FadeCor = StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.2f);
        accumTime = 0f;
        while (accumTime < 2f)
        {
            CanvasGroup.alpha = Mathf.Lerp(0f, 1f, accumTime / 2f);
            yield return 0;
            accumTime += Time.deltaTime;
        }
        CanvasGroup.alpha = 1f;
    }

    public void ExitGame()
    {
        if (Managers.NetworkMng.NumPlayers <= 1)
        {
            Managers.NetworkMng.ExitGame();
        }
        else
        {
            if (isAnimating)
                return;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(GetText(Texts.Text3).DOFade(1f, 1.5f).SetEase(Ease.OutQuad));
            sequence.AppendInterval(0.5f);
            sequence.Append(GetText(Texts.Text3).DOFade(0f, 1.5f).SetEase(Ease.OutQuad));
            sequence.OnComplete(() => isAnimating = false);
            sequence.Play();

            isAnimating = true;
        }
    }
}
