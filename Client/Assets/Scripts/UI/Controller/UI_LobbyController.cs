using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_LobbyController : UI_Base, ILobbyController
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

    enum SubItems
    {
        UI_Loading,
        UI_Lobby,
    }
    #endregion

    #region Fields
    [Header("MENUS")]
    public GameObject mainMenu;
    public GameObject firstMenu;
    public GameObject playMenu;
    public GameObject exitMenu;
    public GameObject extrasMenu;

    [Header("THEME SETTINGS")]
    public Theme theme;
    public enum Theme { custom1, custom2, custom3 };
    private int themeIndex;
    public SlimUI.ModernMenu.ThemedUIData themeController;

    [Header("PANELS")]
    public GameObject mainCanvas;
    public GameObject PanelControls;
    public GameObject PanelVideo;
    public GameObject PanelGame;
    public GameObject PanelKeyBindings;
    public GameObject PanelMovement;
    public GameObject PanelCombat;
    public GameObject PanelGeneral;

    [Header("SETTINGS SCREEN")]
    public GameObject lineGame;
    public GameObject lineVideo;
    public GameObject lineControls;
    public GameObject lineKeyBindings;
    public GameObject lineMovement;
    public GameObject lineCombat;
    public GameObject lineGeneral;
    public GameObject KeyConfirmation;

    [Header("LOADING")]
    public UI_Loading loadingMenu;

    [Header("LOBBY")]
    public UI_Lobby lobbyMenu;
    private Animator CameraObject;
    #endregion

    #region Init
    public override bool Init()
    {

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<UI_Base>(typeof(SubItems));
        DontDestroyOnLoad(gameObject);

        #region Base
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
        #endregion

        loadingMenu = Get<UI_Base>(SubItems.UI_Loading) as UI_Loading;
        loadingMenu.Init();
        loadingMenu.SetInfo(this);

        lobbyMenu = Get<UI_Base>(SubItems.UI_Lobby) as UI_Lobby;
        lobbyMenu.Init();
        lobbyMenu.SetInfo(this);

        foreach (int i in Enum.GetValues(typeof(Buttons)))
        {
            BindEvent(GetButton(i).gameObject, (e) => PlayHover(), Define.UIEvent.PointerEnter);
        }

        playMenu.SetActive(false);
        exitMenu.SetActive(false);
        extrasMenu.SetActive(false);
        firstMenu.SetActive(true);
        mainMenu.SetActive(true);

        PanelVideo.SetActive(false);
        PanelControls.SetActive(false);
        PanelKeyBindings.SetActive(false);
        KeyConfirmation.SetActive(false);
        PanelGame.SetActive(true);

        lineControls.SetActive(false);
        lineKeyBindings.SetActive(false);
        lineVideo.SetActive(false);

        SetThemeColors();

        return true;
    }
    #endregion

    #region Interface
    public void ShowLoadingMenu()
    {
        loadingMenu.gameObject.SetActive(true);
        StartCoroutine(loadingMenu.LoadAsynchronously());
    }

    public void PlayHover()
    {
        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/UI/Click", Define.SoundType.Effect, volume : 0.5f);
    }

    public void DestroyMenu()
    {
        Destroy(gameObject);
    }

    public void ExitMenu()
    {
        playMenu.SetActive(false);
        exitMenu.SetActive(false);
        extrasMenu.SetActive(false);
        firstMenu.SetActive(false);
        loadingMenu.gameObject.SetActive(false);
        PanelVideo.SetActive(false);
        PanelControls.SetActive(false);
        PanelGame.SetActive(false);
        PanelKeyBindings.SetActive(false);
        KeyConfirmation.SetActive(false);
        lineControls.SetActive(false);
        lineKeyBindings.SetActive(false);
        lineVideo.SetActive(false);
    }

    public void OpenRoomCreate()
    {
        lobbyMenu.RoomCreate.gameObject.SetActive(true);
        lobbyMenu.RoomList.SetActive(false);
    }

    public void CloseRoomCreate()
    {
        lobbyMenu.RoomCreate.gameObject.SetActive(false);
        lobbyMenu.RoomList.SetActive(true);
    }

    public void OpenRoomJoin(string sessionName, string password)
    {
        lobbyMenu.RoomJoin.SetInfo(this, sessionName, password);
        lobbyMenu.RoomJoin.gameObject.SetActive(true);
        lobbyMenu.RoomList.SetActive(false);
    }

    public void CloseRoomJoin()
    {
        lobbyMenu.RoomJoin.gameObject.SetActive(false);
        lobbyMenu.RoomList.SetActive(true);
    }
    #endregion

    #region Other
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

    public void PlaySwoosh()
    {
        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/UI/Click2");
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
    #endregion
}
