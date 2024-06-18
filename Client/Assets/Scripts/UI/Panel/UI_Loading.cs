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
    private float _loadingSpeed;
    public KeyCode userPromptKey;
    public bool LoadingMap = false;

    public bool IsMapLoaded { get; private set; } = false;
    public bool isDone { get; private set; } = false;
    public float loadingProgress { get; private set; } = 0.0f;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        GameObject go = Managers.ResourceMng.Instantiate("Cameras/UICamera");
        transform.SetParent(go.transform);

        var canvas = gameObject.GetComponent<Canvas>();
        canvas.sortingOrder = 20;

        DontDestroyOnLoad(transform.parent.gameObject);

        Bind<TMP_Text>(typeof(Texts));
        Bind<Slider>(typeof(Sliders));

        _loadingBar = Get<Slider>(Sliders.ProgressBar);
        _loadPromptText = GetText(Texts.TextPrompt);

        waitForInput = false;
        loadingProgress = 0.0f;
        userPromptKey = KeyCode.F;

        StartCoroutine(LoadAsynchronously());
        if (Managers.SceneMng.CurrentScene.IsSceneType((int)Define.SceneType.LobbyScene))
        {
            _loadingSpeed = 0.3f;
            StartCoroutine(SpawnCheck());
        }
        else if (Managers.SceneMng.CurrentScene.IsSceneType((int)Define.SceneType.ReadyScene))
        {
            _loadingSpeed = 0.1f;
            StartCoroutine(TransitionCheck());
        }

        return true;
    }

    void Update()
    {
        loadingProgress += _loadingSpeed * Time.deltaTime;
    }

    public IEnumerator SpawnCheck()
    {
        yield return new WaitUntil(() => Managers.ObjectMng.MyCreature != null);
        yield return new WaitUntil(() => Managers.ObjectMng.MyCreature.IsSpawned);
        isDone = true;
    }

    public IEnumerator TransitionCheck()
    {
        yield return new WaitUntil(() => Managers.NetworkMng.CurrentPlayState == PlayerSystem.PlayState.Transition);
        yield return new WaitUntil(() => Managers.NetworkMng.CurrentPlayState != PlayerSystem.PlayState.Transition);
        yield return new WaitUntil(() => Managers.ObjectMng.MyCreature != null);
        yield return new WaitUntil(() => Managers.ObjectMng.MyCreature.IsSpawned);
        yield return new WaitUntil(() => IsMapLoaded);
        yield return new WaitForSeconds(2.0f);
        isDone = true;
    }

    public void OnMapLoadComplete()
    {
        IsMapLoaded = true;
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

                if (isDone && Input.GetKeyDown(userPromptKey))
                {
                    OnLoadingDone();
                    yield break;
                }
            }
            else if (isDone && progress >= 0.9f && !waitForInput)
            {
                OnLoadingDone();
                yield break;
            }

            yield return null;
        }

        OnLoadingDone();
    }

    public void OnLoadingDone()
    {
        Managers.UIMng.PanelUI = null;
        Destroy(transform.parent.gameObject);
    }
}
