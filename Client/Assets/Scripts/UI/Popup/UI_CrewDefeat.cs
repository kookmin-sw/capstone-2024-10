using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CrewDefeat : UI_Popup
{
    enum Buttons
    {
        Quit,
    }

    enum Texts
    {
        Text1,
        Text2,
        Text3,
    }

    private CanvasGroup CanvasGroup;
    private Coroutine FadeCor;
    private float accumTime = 0f;
    private float _exitTime = 6f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));

        GetButton((int)Buttons.Quit).onClick.AddListener(ExitGame);

        CanvasGroup = GetComponent<CanvasGroup>();
        StartFadeIn();

        GetText(Texts.Text3).text = $"Automatically exit after {Mathf.Floor(_exitTime)} seconds";
        StartCoroutine(CountDownToExit());

        return true;
    }

    public IEnumerator CountDownToExit()
    {
        float t = -1;
        while ((t = _exitTime - Time.deltaTime) > 0)
        {
            if (Mathf.Floor(t) != Mathf.Floor(_exitTime))
            {
                GetText(Texts.Text3).text = $"Automatically exit after {Mathf.Floor(t)} seconds";
            }
            _exitTime = t;
            yield return null;
        }

        ExitGame();
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
        Managers.NetworkMng.ExitGame();
    }

}
