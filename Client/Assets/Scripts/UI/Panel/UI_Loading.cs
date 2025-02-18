using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : UI_Panel
{
    #region Enums
    public enum Texts
    {
        TextPrompt,
        Content,
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
    public bool loadingMap = false;
    private TMP_Text _tip;

    public bool IsMapLoaded { get; set; } = false;
    public bool IsDone { get; private set; } = false;
    public float LoadingProgress { get; private set; } = 0.0f;
    public bool ShouldWaitForOtherPlayers { get; set; } = false;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        GameObject go = Managers.ResourceMng.Instantiate("Cameras/UICamera");
        transform.SetParent(go.transform);

        var canvas = gameObject.GetComponent<Canvas>();
        canvas.sortingOrder = 20;

        DontDestroyOnLoad(transform.parent.gameObject);
        Managers.SoundMng.MuteOn();

        Bind<TMP_Text>(typeof(Texts));
        Bind<Slider>(typeof(Sliders));


        _loadingBar = Get<Slider>(Sliders.ProgressBar);
        _loadingBar.value = 0;
        _loadPromptText = GetText(Texts.TextPrompt);
        _tip = GetText(Texts.Content);
        _tip.text = Define.TEXT_FOR_TIP[Random.Range(0, Define.TEXT_FOR_TIP.Length)];

        waitForInput = false;
        ShouldWaitForOtherPlayers = true;
        LoadingProgress = 0.0f;
        userPromptKey = KeyCode.F;
        _loadPromptText.text = "";

        StartCoroutine(LoadAsynchronously());
        if (Managers.SceneMng.CurrentScene.IsSceneType((int)Define.SceneType.LobbyScene))
        {
            _loadingSpeed = 0.3f;
            StartCoroutine(SpawnCheck());
        }
        else if (Managers.SceneMng.CurrentScene.IsSceneType((int)Define.SceneType.ReadyScene))
        {
            _loadingSpeed = 0.1f;
            Managers.NetworkMng.IsGameLoading = true;
            StartCoroutine(TransitionCheck());
        }

        return true;
    }

    void Update()
    {
        LoadingProgress += _loadingSpeed * Time.deltaTime;
        _loadingBar.value = Mathf.Clamp01(LoadingProgress / .95f);

        if (Managers.NetworkMng.IsGameLoading && !IsDone)
        {
            OnPlayerDropped();
        }
    }

    public IEnumerator SpawnCheck()
    {
        yield return new WaitUntil(() => Managers.ObjectMng.MyCreature != null);
        yield return new WaitUntil(() => Managers.ObjectMng.MyCreature.IsSpawned);
        IsDone = true;
    }

    public void OnPlayerDropped()
    {
        if (Managers.NetworkMng.IsTutorialScene)
            return;

        if (Managers.NetworkMng.SpawnCount == Define.PLAYER_COUNT)
            return;

        // 게임을 진행할 수 없다고 판단
        Managers.NetworkMng.OnAlienDropped();
        IsDone = true;
        ShouldWaitForOtherPlayers = false;
    }

    public IEnumerator TransitionCheck()
    {
        yield return new WaitUntil(() => Managers.NetworkMng.CurrentPlayState == PlayerSystem.PlayState.Game);
        yield return new WaitUntil(() => Managers.ObjectMng.MyCreature != null);
        yield return new WaitUntil(() => Managers.ObjectMng.MyCreature.IsSpawned);
        yield return new WaitUntil(() => IsMapLoaded);
        yield return new WaitForSeconds(2.0f);
        IsDone = true;
    }

    public IEnumerator LoadAsynchronously()
    {
        while (!IsDone)
        {
            float progress = _loadingBar.value;

            if (progress >= 0.9f && waitForInput)
            {
                _loadingBar.value = 1;

                if (IsDone && Input.GetKeyDown(userPromptKey))
                {
                    OnLoadingDone();
                    yield break;
                }
            }
            else if (IsDone && progress >= 0.9f && !waitForInput)
            {
                OnLoadingDone();
                yield break;
            }

            yield return null;
        }

        OnLoadingDone();
    }

    private IEnumerator WaitProgress()
    {
        _loadingSpeed = 0.4f;
        yield return new WaitUntil(() => _loadingBar.value > 0.9f);
        yield return new WaitForSeconds(0.5f);

        Managers.NetworkMng.IsGameLoading = false;

        yield return new WaitUntil(() => ShouldWaitForOtherPlayers == false);

        StopAllCoroutines();
        Managers.UIMng.PanelUI = null;
        Managers.SoundMng.MuteOff();
        Destroy(transform.parent.gameObject);
    }

    public void OnLoadingDone()
    {
        StartCoroutine(WaitProgress());
    }
}
