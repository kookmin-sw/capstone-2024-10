using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Notification : UI_Base
{
    enum Texts
    {
        Message,
        Notification
    }
    enum Images
    {
        //Fill
        Detail1,
        Detail2,
    }

    private CanvasGroup CanvasGroup;
    private Coroutine FadeCor;
    private float accumTime = 0f;
    private float duration = 1.0f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TMP_Text>(typeof(Texts));
        Bind<Image>(typeof(Images));

        CanvasGroup = GetComponent<CanvasGroup>();

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

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(3.0f);
        accumTime = 0f;
        while (accumTime < 2f)
        {
            CanvasGroup.alpha = Mathf.Lerp(1f, 0f, accumTime / 2f);
            yield return 0;
            accumTime += Time.deltaTime;
        }
        CanvasGroup.alpha = 0f;
    }

    private bool _scrolldown = false;

    private void ScrollDown()
    {
        if (_scrolldown)
            return;

        var ui = GetComponent<RectTransform>();
        Vector2 startPosition = new Vector2(0, ui.rect.height / 2 + 20);
        ui.anchoredPosition = startPosition;

        Vector2 targetPosition = new Vector2(0, - ui.rect.height / 2 - 20);
        ui.DOAnchorPos(targetPosition, duration).SetEase(Ease.InQuad);
        DOVirtual.DelayedCall(duration + 1, () =>
        {
            if (Managers.NetworkMng.ServerFound)
                HideServerConnectMessage();
        });
        _scrolldown = true;
    }

    private void ScrollUp()
    {
        if (!_scrolldown)
            return;

        var ui = GetComponent<RectTransform>();
        Vector2 startPosition = new Vector2(0, - ui.rect.height / 2 -20);
        ui.anchoredPosition = startPosition;

        Vector2 targetPosition = new Vector2(0, ui.rect.height / 2 + 20);
        ui.DOAnchorPos(targetPosition, duration).SetEase(Ease.InQuad);
        _scrolldown = false;
    }

    public void EscapeMessage()
    {
        GetText(Texts.Message).text = "One of the Crew has Escaped";
        StartFadeIn();
    }
    public void DeadMessage()
    {
        GetText(Texts.Message).text = "One of the Crew has Died";
        StartFadeIn();
    }
    public void DropMessage()
    {
        GetText(Texts.Message).text = "One of the Crew has Left the Game";
        StartFadeIn();
    }

    public void ShowServerConnectMessage()
    {
        CanvasGroup.alpha = 1f;
        GetText(Texts.Message).text = "Searching for the nearest game server";
        ScrollDown();
    }

    public void HideServerConnectMessage()
    {
        CanvasGroup.alpha = 1f;
        GetText(Texts.Message).text = "Server found";
        ScrollUp();
    }

    public void Hide() => gameObject.SetActive(false);
}

