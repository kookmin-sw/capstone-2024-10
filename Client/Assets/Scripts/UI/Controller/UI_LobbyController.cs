using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LobbyController : UI_Base
{
    #region Enums
    enum GameObjects
    {
        //Menus
        MAIN,
        PLAY,
        EXIT,
        EXTRAS,
        //Panels
        Canv_Main,
        PanelControls,
        PanelVideo,
        PanelGame,
        PanelKeyBindings,
        MovementPanel,
        CombatPanel,
        GeneralPanel,
        //SettingsScreen
        LineGame,
        LineVideo,
        LineControls,
        LineKeyBindings,
        LineMovement,
        LineCombat,
        LineGeneral,
        KeyConfirmation,
        // LoadingScreen
        Loading,
        ProgressBar,
        TextPrompt,
    }

    enum Buttons
    {
        Btn_PlayCampaign,
        Btn_Settings,
        Btn_Extras,
        Btn_Exit,
        Btn_No1,
        Btn_Yes,
        Btn_CCP,
        Btn_Clean1,
        Btn_Essence,
        Btn_SciFi,
        Combat_Btn,
        Movement_Btn,
        General_Btn,
        Btn_Assign1,
        Btn_Assign2,
        Btn_Assign3,
        Btn_Assign4,
        Btn_Assign5,
        Btn_Assign6,
        Btn_Assign7,
        Btn_Assign8,
        Btn_Assign9,
        Btn_Assign10,
        Btn_Assign11,
        Btn_Return,
        Btn_Game,
        Btn_Controls,
        Btn_Video,
        KeyBindings_Btn,
        Btn_No2,
    }

    enum Others
    {
        AA2X_Btn,
        AA4X_Btn,
        AA8X_Btn,
        AAOff_Btn,
        AmbientOcclusion_Btn,
        CameraEffects_Btn,
        MotionBlur_Btn,
        FullScreen_Btn,
        HardDif_Btn,
        Inverse_Btn,
        NormDif_Btn,
        ShadowOff_Btn,
        ShadowLow_Btn,
        ShadowHigh_Btn,
        ShowHud_Btn,
        TextureLow_Btn,
        TextureMed_Btn,
        TextureHigh_Btn,
        ToolTip_Btn,
        VSync_Btn,
    }

    enum Sliders
    {
        MusicSlider,
        SensitivityXSlider,
        SensitivityYSlider,
        SmoothingSlider,
    }
    #endregion

    #region Fields
    // campaign button sub menu
    [Header("MENUS")]
    [Tooltip("The Menu for when the MAIN menu buttons")]
    public GameObject mainMenu;
    [Tooltip("THe first list of buttons")]
    public GameObject firstMenu;
    [Tooltip("The Menu for when the PLAY button is clicked")]
    public GameObject playMenu;
    [Tooltip("The Menu for when the EXIT button is clicked")]
    public GameObject exitMenu;
    [Tooltip("Optional 4th Menu")]
    public GameObject extrasMenu;

    public enum Theme { custom1, custom2, custom3 };
    [Header("THEME SETTINGS")]
    public Theme theme;
    private int themeIndex;
    public SlimUI.ModernMenu.ThemedUIData themeController;

    [Header("PANELS")]
    [Tooltip("The UI Panel parenting all sub menus")]
    public GameObject mainCanvas;
    [Tooltip("The UI Panel that holds the CONTROLS window tab")]
    public GameObject PanelControls;
    [Tooltip("The UI Panel that holds the VIDEO window tab")]
    public GameObject PanelVideo;
    [Tooltip("The UI Panel that holds the GAME window tab")]
    public GameObject PanelGame;
    [Tooltip("The UI Panel that holds the KEY BINDINGS window tab")]
    public GameObject PanelKeyBindings;
    [Tooltip("The UI Sub-Panel under KEY BINDINGS for MOVEMENT")]
    public GameObject PanelMovement;
    [Tooltip("The UI Sub-Panel under KEY BINDINGS for COMBAT")]
    public GameObject PanelCombat;
    [Tooltip("The UI Sub-Panel under KEY BINDINGS for GENERAL")]
    public GameObject PanelGeneral;


    // highlights in settings screen
    [Header("SETTINGS SCREEN")]
    [Tooltip("Highlight Image for when GAME Tab is selected in Settings")]
    public GameObject lineGame;
    [Tooltip("Highlight Image for when VIDEO Tab is selected in Settings")]
    public GameObject lineVideo;
    [Tooltip("Highlight Image for when CONTROLS Tab is selected in Settings")]
    public GameObject lineControls;
    [Tooltip("Highlight Image for when KEY BINDINGS Tab is selected in Settings")]
    public GameObject lineKeyBindings;
    [Tooltip("Highlight Image for when MOVEMENT Sub-Tab is selected in KEY BINDINGS")]
    public GameObject lineMovement;
    [Tooltip("Highlight Image for when COMBAT Sub-Tab is selected in KEY BINDINGS")]
    public GameObject lineCombat;
    [Tooltip("Highlight Image for when GENERAL Sub-Tab is selected in KEY BINDINGS")]
    public GameObject lineGeneral;
    public GameObject KeyConfirmation;

    [Header("LOADING SCREEN")]
    [Tooltip("If this is true, the loaded scene won't load until receiving user input")]
    public bool waitForInput = true;
    public GameObject loadingMenu;
    [Tooltip("The loading bar Slider UI element in the Loading Screen")]
    public Slider loadingBar;
    public TMP_Text loadPromptText;
    public KeyCode userPromptKey;

    private Animator CameraObject;
    #endregion

    #region Init
    public override bool Init()
    {

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));

        CameraObject = Camera.main.GetComponent<Animator>();
        mainMenu = gameObject;
        firstMenu = GetObject((int)GameObjects.MAIN);
        playMenu = GetObject((int)GameObjects.PLAY);
        exitMenu = GetObject((int)GameObjects.EXIT);
        extrasMenu = GetObject((int)GameObjects.EXTRAS);

        theme = Theme.custom2;
        themeController = Managers.ResourceMng.Load<SlimUI.ModernMenu.ThemedUIData>("Scriptable/ThemeSettings_ModernMenuUI");

        mainCanvas = GetObject((int)GameObjects.Canv_Main);
        PanelControls = GetObject((int)GameObjects.PanelControls);
        PanelVideo = GetObject((int)GameObjects.PanelVideo);
        PanelGame = GetObject((int)GameObjects.PanelGame);
        PanelKeyBindings = GetObject((int)GameObjects.PanelKeyBindings);
        PanelMovement = GetObject((int)GameObjects.MovementPanel);
        PanelCombat = GetObject((int)GameObjects.CombatPanel);
        PanelGeneral = GetObject((int)GameObjects.GeneralPanel);

        lineGame = GetObject((int)GameObjects.LineGame);
        lineVideo = GetObject((int)GameObjects.LineVideo);
        lineControls = GetObject((int)GameObjects.LineControls);
        lineKeyBindings = GetObject((int)GameObjects.LineKeyBindings);
        lineMovement = GetObject((int)GameObjects.LineMovement);
        lineCombat = GetObject((int)GameObjects.LineCombat);
        lineGeneral = GetObject((int)GameObjects.LineGeneral);
        KeyConfirmation = GetObject((int)GameObjects.KeyConfirmation);

        loadingMenu = GetObject((int)GameObjects.Loading);
        loadingBar = GetObject((int)GameObjects.ProgressBar).GetComponent<Slider>();
        loadPromptText = GetObject((int)GameObjects.TextPrompt).GetComponent<TMP_Text>();
        userPromptKey = KeyCode.Space;

        foreach (int i in Enum.GetValues(typeof(Buttons)))
        {
            BindEvent(GetButton(i).gameObject, (e) => PlayHover(), Define.UIEvent.PointerEnter);
        }

        GetButton((int)Buttons.Btn_PlayCampaign).onClick.AddListener(PlayCampaign);
        GetButton((int)Buttons.Btn_Settings).onClick.AddListener(Position2);
        GetButton((int)Buttons.Btn_Settings).onClick.AddListener(ReturnMenu);
        GetButton((int)Buttons.Btn_Settings).onClick.AddListener(PlaySwoosh);

        GetButton((int)Buttons.Btn_Exit).onClick.AddListener(AreYouSure);
        GetButton((int)Buttons.Btn_Extras).onClick.AddListener(ExtrasMenu);
        GetButton((int)Buttons.Btn_Game).onClick.AddListener(GamePanel);
        GetButton((int)Buttons.Btn_No1).onClick.AddListener(ReturnMenu);
        GetButton((int)Buttons.Btn_PlayCampaign).onClick.AddListener(PlayCampaign);
        GetButton((int)Buttons.Btn_Return).onClick.AddListener(Position1);
        GetButton((int)Buttons.Btn_Yes).onClick.AddListener(QuitGame);
        GetButton((int)Buttons.Combat_Btn).onClick.AddListener(CombatPanel);
        GetButton((int)Buttons.General_Btn).onClick.AddListener(GeneralPanel);
        GetButton((int)Buttons.KeyBindings_Btn).onClick.AddListener(KeyBindingsPanel);
        GetButton((int)Buttons.Movement_Btn).onClick.AddListener(MovementPanel);



        GetButton((int)Buttons.Btn_Video).onClick.AddListener(VideoPanel);
        GetButton((int)Buttons.Btn_Controls).onClick.AddListener(ControlsPanel);
        GetButton((int)Buttons.KeyBindings_Btn).onClick.AddListener(KeyBindingsPanel);
        GetButton((int)Buttons.Btn_Return).onClick.AddListener(ReturnMenu);

        var lobby = mainCanvas.AddComponent<UI_Lobby>();
        lobby.Init();
        lobby.SetInfo(this);

        playMenu.SetActive(false);
        exitMenu.SetActive(false);
        extrasMenu.SetActive(false);
        firstMenu.SetActive(true);
        mainMenu.SetActive(true);
        loadingMenu.SetActive(false);

        PanelVideo.SetActive(false);
        PanelControls.SetActive(false);
        PanelGame.SetActive(true);
        PanelKeyBindings.SetActive(false);
        KeyConfirmation.SetActive(false);

        lineControls.SetActive(false);
        lineKeyBindings.SetActive(false);
        lineVideo.SetActive(false);

        SetThemeColors();

        return true;
    }
    #endregion

    void SetThemeColors()
    {
        switch (theme)
        {
            case Theme.custom1:
                themeController.currentColor = themeController.custom1.graphic1;
                themeController.textColor = themeController.custom1.text1;
                themeIndex = 0;
                break;
            case Theme.custom2:
                themeController.currentColor = themeController.custom2.graphic2;
                themeController.textColor = themeController.custom2.text2;
                themeIndex = 1;
                break;
            case Theme.custom3:
                themeController.currentColor = themeController.custom3.graphic3;
                themeController.textColor = themeController.custom3.text3;
                themeIndex = 2;
                break;
            default:
                Debug.Log("Invalid theme selected.");
                break;
        }
    }

    public void PlayCampaign()
    {
        exitMenu.SetActive(false);
        if (extrasMenu) extrasMenu.SetActive(false);
        playMenu.SetActive(true);
    }

    public void PlayCampaignMobile()
    {
        exitMenu.SetActive(false);
        if (extrasMenu) extrasMenu.SetActive(false);
        playMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void ReturnMenu()
    {
        playMenu.SetActive(false);
        if (extrasMenu) extrasMenu.SetActive(false);
        exitMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void LoadScene(string scene)
    {
        if (scene != "")
        {
            StartCoroutine(LoadAsynchronously(scene));
        }
    }

    public void DisablePlayCampaign()
    {
        playMenu.SetActive(false);
    }

    public void Position2()
    {
        DisablePlayCampaign();
        CameraObject.SetFloat("Animate", 1);
    }

    public void Position1()
    {
        CameraObject.SetFloat("Animate", 0);
    }

    void DisablePanels()
    {
        PanelControls.SetActive(false);
        PanelVideo.SetActive(false);
        PanelGame.SetActive(false);
        PanelKeyBindings.SetActive(false);

        lineGame.SetActive(false);
        lineControls.SetActive(false);
        lineVideo.SetActive(false);
        lineKeyBindings.SetActive(false);

        PanelMovement.SetActive(false);
        lineMovement.SetActive(false);
        PanelCombat.SetActive(false);
        lineCombat.SetActive(false);
        PanelGeneral.SetActive(false);
        lineGeneral.SetActive(false);
    }

    public void GamePanel()
    {
        DisablePanels();
        PanelGame.SetActive(true);
        lineGame.SetActive(true);
    }

    public void VideoPanel()
    {
        DisablePanels();
        PanelVideo.SetActive(true);
        lineVideo.SetActive(true);
    }

    public void ControlsPanel()
    {
        DisablePanels();
        PanelControls.SetActive(true);
        lineControls.SetActive(true);
    }

    public void KeyBindingsPanel()
    {
        DisablePanels();
        MovementPanel();
        PanelKeyBindings.SetActive(true);
        lineKeyBindings.SetActive(true);
    }

    public void MovementPanel()
    {
        DisablePanels();
        PanelKeyBindings.SetActive(true);
        PanelMovement.SetActive(true);
        lineMovement.SetActive(true);
    }

    public void CombatPanel()
    {
        DisablePanels();
        PanelKeyBindings.SetActive(true);
        PanelCombat.SetActive(true);
        lineCombat.SetActive(true);
    }

    public void GeneralPanel()
    {
        DisablePanels();
        PanelKeyBindings.SetActive(true);
        PanelGeneral.SetActive(true);
        lineGeneral.SetActive(true);
    }

    public void PlayHover()
    {
        Managers.SoundMng.Play("Music/Clicks/SFX_Click_Mechanical", Define.SoundType.Effect, 0.1f);
    }

    public void PlaySFXHover()
    {
        Managers.SoundMng.Play("Music/Clicks/SFX_Click_Punch");
    }

    public void PlaySwoosh()
    {
        Managers.SoundMng.Play("Music/Clicks/SFX_Click_Whoosh");
    }

    // Are You Sure - Quit Panel Pop Up
    public void AreYouSure()
    {
        exitMenu.SetActive(true);
        if (extrasMenu) extrasMenu.SetActive(false);
        DisablePlayCampaign();
    }

    public void AreYouSureMobile()
    {
        exitMenu.SetActive(true);
        if (extrasMenu) extrasMenu.SetActive(false);
        mainMenu.SetActive(false);
        DisablePlayCampaign();
    }

    public void ExtrasMenu()
    {
        playMenu.SetActive(false);
        if (extrasMenu) extrasMenu.SetActive(true);
        exitMenu.SetActive(false);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
    }

    // Load Bar synching animation
    IEnumerator LoadAsynchronously(string sceneName)
    { // scene name is just the name of the current scene being loaded
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;
        mainCanvas.SetActive(false);
        loadingMenu.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .95f);
            loadingBar.value = progress;

            if (operation.progress >= 0.9f && waitForInput)
            {
                loadPromptText.text = "Press " + userPromptKey.ToString().ToUpper() + " to continue";
                loadingBar.value = 1;

                if (Input.GetKeyDown(userPromptKey))
                {
                    operation.allowSceneActivation = true;
                }
            }
            else if (operation.progress >= 0.9f && !waitForInput)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
