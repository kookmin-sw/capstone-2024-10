using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_AlienWin : UI_Panel
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

    private CanvasGroup _canvasGroup;
    private Coroutine _fadeCor;
    private float _accumTime = 0f;
    private bool _isAnimating = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));

        GetButton((int)Buttons.Quit).onClick.AddListener(ExitGame);
        GetText(Texts.Text3).alpha = 0f;

        _canvasGroup = GetComponent<CanvasGroup>();
        StartFadeIn();
        return true;
    }

    public void StartFadeIn()
    {
        if (_fadeCor != null)
        {
            StopAllCoroutines();
            _fadeCor = null;
        }
        _fadeCor = StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.2f);
        _accumTime = 0f;
        while (_accumTime < 2f)
        {
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, _accumTime / 2f);
            yield return 0;
            _accumTime += Time.deltaTime;
        }
        _canvasGroup.alpha = 1f;
    }

    public void ExitGame()
    {
        if (Managers.NetworkMng.NumPlayers <= 1)
        {
            Managers.NetworkMng.ExitGame();
        }
        else
        {
            if (_isAnimating)
                return;

            Sequence sequence = DOTween.Sequence();

            sequence.Append(GetText(Texts.Text3).DOFade(1f, 1.5f).SetEase(Ease.OutQuad));
            sequence.AppendInterval(0.5f);
            sequence.Append(GetText(Texts.Text3).DOFade(0f, 1.5f).SetEase(Ease.OutQuad));
            sequence.OnComplete(() => _isAnimating = false);
            sequence.Play();

            _isAnimating = true;
        }
    }
}
