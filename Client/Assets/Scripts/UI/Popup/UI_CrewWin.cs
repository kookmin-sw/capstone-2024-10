using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrewWin : UI_Panel
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

    private CanvasGroup _canvasGroup;
    private Coroutine _fadeCor;
    private float _accumTime = 0f;
    private float _exitTime = Define.EXIT_TIME + 1;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));

        GetButton((int)Buttons.Quit).onClick.AddListener(ExitGame);

        _canvasGroup = GetComponent<CanvasGroup>();
        StartFadeIn();

        GetText(Texts.Text3).text = $"Automatically leave after {Mathf.Floor(_exitTime)} seconds";
        StartCoroutine(CountDownToExit());

        return true;
    }

    public IEnumerator CountDownToExit()
    {
        float t = -1;
        while ((t = _exitTime - Time.deltaTime) > 0)
        {
            if (Managers.UIMng.SceneUI != null)
            {
                Destroy(Managers.UIMng.SceneUI.gameObject);
                Managers.UIMng.SceneUI = null;
            }

            if (Mathf.Floor(t) != Mathf.Floor(_exitTime))
            {
                GetText(Texts.Text3).text = $"Automatically leave after {Mathf.Floor(t)} seconds";
            }
            _exitTime = t;
            yield return null;
        }

        ExitGame();
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
        Managers.NetworkMng.ExitGame();
    }
}
