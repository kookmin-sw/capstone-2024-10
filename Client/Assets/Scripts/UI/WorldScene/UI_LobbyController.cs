using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UI_LobbyController : UI_Base
{
    #region Enums

    enum GameObjects
    {
        MAIN,
        PLAY,
        EXIT,
        EXTRAS,
        TITLE
    }

    enum Buttons
    {
        Btn_PlayCampaign,
        Btn_Settings,
        Btn_Manual,
        Btn_Credit,
        Btn_Exit,
        Btn_No1,
        Btn_Yes,
        Btn_Return,
    }

    public enum Texts
    {
        Version,
    }

    #endregion

    #region Fields
    private Animator _cameraAnimator;
    private UI_Loading _loadingMenu;
    private UI_CreditController _creditController;
    #endregion

    #region Init
    public override bool Init()
    {
        if (!base.Init())
            return false;

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Text>(typeof(Texts));
        DontDestroyOnLoad(gameObject);

        #region Base
        _cameraAnimator = Camera.main.GetComponent<Animator>();
        _creditController = FindObjectOfType<UI_CreditController>();

        GetButton(Buttons.Btn_PlayCampaign).onClick.AddListener(PlayCampaign);
        GetButton(Buttons.Btn_Settings).onClick.AddListener(Position2);
        GetButton(Buttons.Btn_Settings).onClick.AddListener(ReturnMenu);
        GetButton(Buttons.Btn_Settings).onClick.AddListener(PlaySwoosh);

        GetButton(Buttons.Btn_Credit).onClick.AddListener(Position4);

        GetButton(Buttons.Btn_Exit).onClick.AddListener(AreYouSure);
        GetButton(Buttons.Btn_Manual).onClick.AddListener(Position3);
        GetButton(Buttons.Btn_Manual).onClick.AddListener(PlaySwoosh);
        GetButton(Buttons.Btn_No1).onClick.AddListener(ReturnMenu);
        GetButton(Buttons.Btn_PlayCampaign).onClick.AddListener(PlayCampaign);
        GetButton(Buttons.Btn_Return).onClick.AddListener(Position1);
        GetButton(Buttons.Btn_Yes).onClick.AddListener(QuitGame);
        GetButton(Buttons.Btn_Return).onClick.AddListener(ReturnMenu);
        #endregion

        var popup = Managers.UIMng.ShowPopupUI<UI_Lobby>(parent : GetObject(GameObjects.PLAY).transform);
        popup.Init();
        popup.SetInfo(this);

        foreach (int i in Enum.GetValues(typeof(Buttons)))
        {
            BindEvent(GetButton(i).gameObject, (e) => PlayHover(), Define.UIEvent.PointerEnter);
        }

        GetObject(GameObjects.PLAY).SetActive(false);
        GetObject(GameObjects.EXIT).SetActive(false);
        if (GetObject(GameObjects.EXTRAS)) GetObject(GameObjects.EXTRAS).SetActive(false);
        GetObject(GameObjects.MAIN).SetActive(true);
        GetObject(GameObjects.TITLE).SetActive(true);


        //GetText(Texts.Version).text = "v" + PlayerSettings.bundleVersion;
        // 빌드 시 사용 할 수 있음
        GetText(Texts.Version).text = "v" + Application.version;

        return true;
    }
#endregion

    #region Other
    public void PlayHover()
    {
        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/UI/Click", Define.SoundType.Effect, volume : 0.5f);
    }

    public void DestroyMenu()
    {
        Destroy(gameObject);
    }

    public void PlayCampaign()
    {
        GetObject(GameObjects.EXIT).SetActive(false);
        if (GetObject(GameObjects.EXTRAS)) GetObject(GameObjects.EXTRAS).SetActive(false);
        GetObject(GameObjects.PLAY).SetActive(true);
        GetObject(GameObjects.TITLE).SetActive(false);
    }

    public void PlayCampaignMobile()
    {
        GetObject(GameObjects.EXIT).SetActive(false);
        if (GetObject(GameObjects.EXTRAS)) GetObject(GameObjects.EXTRAS).SetActive(false);
        GetObject(GameObjects.PLAY).SetActive(true);
        GetObject(GameObjects.MAIN).SetActive(false);
        GetObject(GameObjects.TITLE).SetActive(false);
    }

    public void ReturnMenu()
    {
        GetObject(GameObjects.PLAY).SetActive(false);
        if (GetObject(GameObjects.EXTRAS)) GetObject(GameObjects.EXTRAS).SetActive(false);
        GetObject(GameObjects.EXIT).SetActive(false);
        GetObject(GameObjects.MAIN).SetActive(true);
        GetObject(GameObjects.TITLE).SetActive(true);
    }


    public void DisablePlayCampaign()
    {
        GetObject(GameObjects.PLAY).SetActive(false);
        GetObject(GameObjects.TITLE).SetActive(true);
    }
    public void Position4()
    {
        DisablePlayCampaign();
        _cameraAnimator.SetFloat("Animate", 3);
        _creditController.StartScrollCredits();
    }

    public void Position3()
    {
        DisablePlayCampaign();
        _cameraAnimator.SetFloat("Animate", 2);
    }

    public void Position2()
    {
        DisablePlayCampaign();
        _cameraAnimator.SetFloat("Animate", 1);
    }

    public void Position1()
    {
        GetObject(GameObjects.TITLE).SetActive(true);
        GetObject(GameObjects.EXIT).SetActive(false);
        _cameraAnimator.SetFloat("Animate", 0);
        _creditController.ResetCredit();
        StopAllCoroutines();
    }

    public void PlaySwoosh()
    {
        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/UI/Click2", Define.SoundType.Facility, isOneShot:true);
    }

    // Are You Sure - Quit Panel Pop Up
    public void AreYouSure()
    {
        GetObject(GameObjects.TITLE).SetActive(false);
        GetObject(GameObjects.EXIT).SetActive(true);
        if (GetObject(GameObjects.EXTRAS)) GetObject(GameObjects.EXTRAS).SetActive(false);
        GetObject(GameObjects.PLAY).SetActive(false);
    }

    public void AreYouSureMobile()
    {
        GetObject(GameObjects.EXIT).SetActive(true);
        if (GetObject(GameObjects.EXTRAS)) GetObject(GameObjects.EXTRAS).SetActive(false);
        GetObject(GameObjects.MAIN).SetActive(false);
        DisablePlayCampaign();
    }

    public void ExtrasMenu()
    {
        GetObject(GameObjects.PLAY).SetActive(false);
        if (GetObject(GameObjects.EXTRAS)) GetObject(GameObjects.EXTRAS).SetActive(true);
        GetObject(GameObjects.EXIT).SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
    }
    #endregion
}
