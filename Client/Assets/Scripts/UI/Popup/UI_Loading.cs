using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : UI_Base
{
    #region Enums
    public enum Texts
    {
        TextPrompt,
    }

    public enum Sliders
    {
        ProgressBar
    }
    #endregion

    public bool waitForInput;
    public GameObject loadingMenu;
    public Slider loadingBar;
    public TMP_Text loadPromptText;
    public KeyCode userPromptKey;

    private ILobbyController _controller;

    public bool isDone { get; private set; } = false;
    public float loadingProgress { get; private set; } = 0.0f;

    public override bool Init()
    {
        if (_init == true)
            return true;

        if (base.Init() == false)
            return false;

        Bind<TMP_Text>(typeof(Texts));
        Bind<Slider>(typeof(Sliders));

        loadingBar = Get<Slider>(Sliders.ProgressBar);
        loadPromptText = GetText(Texts.TextPrompt);

        waitForInput = false;
        loadingProgress = 0.0f;
        userPromptKey = KeyCode.F;

        return true;
    }

    void Update()
    {
        loadingProgress += 0.5f * Time.deltaTime;
        if (Managers.SceneMng.CurrentScene.SceneType == Define.SceneType.ReadyScene)
        {
            if (Managers.ObjectMng.MyCreature)
            {
                OnLoadingDone();
            }
        }
    }

    public void SetInfo(ILobbyController controller)
    {
        _controller = controller;
        gameObject.SetActive(false);
    }

    public IEnumerator LoadAsynchronously()
    {
        while (!isDone)
        {
            float progress = Mathf.Clamp01(loadingProgress / .95f);
            loadingBar.value = progress;

            if (progress >= 0.9f && waitForInput)
            {
                loadPromptText.text = "Press " + userPromptKey.ToString().ToUpper() + " to continue";
                loadingBar.value = 1;

                if (Input.GetKeyDown(userPromptKey))
                {
                    OnLoadingDone();
                }
            }
            else if (progress >= 0.9f && !waitForInput)
            {
                OnLoadingDone();
            }

            yield return null;
        }
    }

    public void OnLoadingDone()
    {
        isDone = true;
        gameObject.SetActive(false);
        _controller.DestroyMenu();
    }
}
