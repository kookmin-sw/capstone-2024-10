using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : UI_Panel
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
    private Slider _loadingBar;
    private TMP_Text _loadPromptText;
    public KeyCode userPromptKey;

    public bool isDone { get; private set; } = false;
    public float loadingProgress { get; private set; } = 0.0f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        DontDestroyOnLoad(transform.parent.gameObject);

        Bind<TMP_Text>(typeof(Texts));
        Bind<Slider>(typeof(Sliders));

        _loadingBar = Get<Slider>(Sliders.ProgressBar);
        _loadPromptText = GetText(Texts.TextPrompt);

        waitForInput = false;
        loadingProgress = 0.0f;
        userPromptKey = KeyCode.F;

        StartCoroutine(LoadAsynchronously());

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

    public IEnumerator LoadAsynchronously()
    {
        while (!isDone)
        {
            float progress = Mathf.Clamp01(loadingProgress / .95f);
            _loadingBar.value = progress;

            if (progress >= 0.9f && waitForInput)
            {
                _loadPromptText.text = "Press " + userPromptKey.ToString().ToUpper() + " to continue";
                _loadingBar.value = 1;

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
        Destroy(transform.parent.gameObject);
    }
}
