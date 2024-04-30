using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Kill : UI_Popup
{
    enum Buttons
    {
        Quit,
    }

    enum Texts
    {
        Text1,
        Text2
    }

    private CanvasGroup CanvasGroup;
    private Coroutine FadeCor;
    private float accumTime = 0f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));

        GetButton((int)Buttons.Quit).onClick.AddListener(ExitGame);

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
        while (accumTime < 1f)
        {
            CanvasGroup.alpha = Mathf.Lerp(0f, 1f, accumTime / 1f);
            yield return 0;
            accumTime += Time.deltaTime;
        }
        CanvasGroup.alpha = 1f;
    }

    public void ExitGame()
    {
        Managers.StartMng.Player.ExitGame();
    }
}
