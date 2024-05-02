using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;

public class UI_SettingController : UI_Base
{
    #region Enums
    enum GameObjects
    {
        fullscreentext,
        ambientocclusiontext,
        OFFLINE,
        LOWLINE,
        HIGHLINE,
        OFFMSLINE,
        _2XMSLINE,
        _4XMSLINE,
        _8XMSLINE,
        vsynctext,
        motionblurtext,
        LOWLINE1,
        MEDLINE1,
        HIGHLINE1,
        cameraeffectstext,
        showhudtext,
        tooltiptext,
        normaldifficultytext,
        NORMALINE,
        hardcoredifficultytext,
        HARDCORELINE,
        invertmousetext,
        PanelControls,
        PanelVideo,
        PanelGame,
        PanelKeyBindings,
        LineGame,
        LineVideo,
        LineControls,
        LineKeyBindings,
        LineMovement,
        LineCombat,
        LineGeneral,
        KeyConfirmation,
        MovementPanel,
        CombatPanel,
        GeneralPanel,
    }

    enum Buttons
    {
        Btn_Return,
        Btn_Game,
        Btn_Controls,
        Btn_Video,
        KeyBindings_Btn,
        Btn_No2,
        Inverse_Btn,
        FullScreen_Btn,
        AmbientOcclusion_Btn,
        ShadowOff_Btn,
        ShadowLow_Btn,
        ShadowHigh_Btn,
        VSync_Btn,
        MotionBlur_Btn,
        TextureLow_Btn,
        TextureMed_Btn,
        TextureHigh_Btn,
        AAOff_Btn,
        AA2X_Btn,
        AA4X_Btn,
        AA8X_Btn,
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
    }

    enum Dropdowns
    {
        Dropdown,
    }

    enum Sliders
    {
        SensitivityXSlider,
        SensitivityYSlider,
        SmoothingSlider,
        MusicSlider,
    }

    #endregion

    private float _sliderValue = 0.0f;
    private float _sliderValueXSensitivity = 0.0f;
    private float _sliderValueYSensitivity = 0.0f;
    private float _sliderValueSmoothing = 0.0f;

    UI_LobbyController _controller;

    public override bool Init()
    {
        if (!base.Init())
            return false;

        Bind<GameObject>(typeof(GameObjects));
        Bind<Button>(typeof(Buttons));
        Bind<TMP_Dropdown>(typeof(Dropdowns));
        Bind<Slider>(typeof(Sliders));

        _controller = FindObjectOfType<UI_LobbyController>();

        for (int i = 0; i < Enum.GetNames(typeof(Buttons)).Length; i++)
        {
            GetButton(i).gameObject.BindEvent((e) => { _controller.PlayHover(); }, Define.UIEvent.PointerEnter);
        }

        GetButton(Buttons.Btn_Return).gameObject.BindEvent((e) => { _controller.Position1(); }, Define.UIEvent.Click);
        GetButton(Buttons.Btn_Game).gameObject.BindEvent((e) => { GamePanel(); }, Define.UIEvent.Click);
        GetButton(Buttons.Btn_Controls).gameObject.BindEvent((e) => { ControlsPanel(); }, Define.UIEvent.Click);
        GetButton(Buttons.Btn_Video).gameObject.BindEvent((e) => { VideoPanel(); }, Define.UIEvent.Click);
        GetButton(Buttons.KeyBindings_Btn).gameObject.BindEvent((e) => { KeyBindingsPanel(); }, Define.UIEvent.Click);

        GetButton(Buttons.Combat_Btn).onClick.AddListener(CombatPanel);
        GetButton(Buttons.General_Btn).onClick.AddListener(GeneralPanel);
        GetButton(Buttons.Movement_Btn).onClick.AddListener(MovementPanel);

        Get<Slider>(Sliders.MusicSlider).gameObject.BindEvent((e) => { MusicSlider(); }, Define.UIEvent.Drag);
        Get<Slider>(Sliders.SensitivityXSlider).gameObject.BindEvent((e) => { SensitivityXSlider(); }, Define.UIEvent.Drag);
        Get<Slider>(Sliders.SensitivityYSlider).gameObject.BindEvent((e) => { SensitivityYSlider(); }, Define.UIEvent.Drag);
        Get<Slider>(Sliders.SmoothingSlider).gameObject.BindEvent((e) => { SensitivitySmoothing(); }, Define.UIEvent.Drag);

        GetButton(Buttons.Inverse_Btn).gameObject.BindEvent((e) => { InvertMouse(); }, Define.UIEvent.Click);
        GetButton(Buttons.FullScreen_Btn).gameObject.BindEvent((e) => { FullScreen(); }, Define.UIEvent.Click);
        GetButton(Buttons.AmbientOcclusion_Btn).gameObject.BindEvent((e) => { AmbientOcclusion(); }, Define.UIEvent.Click);
        GetButton(Buttons.ShadowOff_Btn).gameObject.BindEvent((e) => { ShadowsOff(); }, Define.UIEvent.Click);
        GetButton(Buttons.ShadowLow_Btn).gameObject.BindEvent((e) => { ShadowsLow(); }, Define.UIEvent.Click);
        GetButton(Buttons.ShadowHigh_Btn).gameObject.BindEvent((e) => { ShadowsHigh(); }, Define.UIEvent.Click);
        GetButton(Buttons.VSync_Btn).gameObject.BindEvent((e) => { vsync(); }, Define.UIEvent.Click);
        GetButton(Buttons.MotionBlur_Btn).gameObject.BindEvent((e) => { MotionBlur(); }, Define.UIEvent.Click);
        GetButton(Buttons.TextureLow_Btn).gameObject.BindEvent((e) => { TexturesLow(); }, Define.UIEvent.Click);
        GetButton(Buttons.TextureMed_Btn).gameObject.BindEvent((e) => { TexturesMed(); }, Define.UIEvent.Click);
        GetButton(Buttons.TextureHigh_Btn).gameObject.BindEvent((e) => { TexturesHigh(); }, Define.UIEvent.Click);

        // check difficulty
        if (PlayerPrefs.GetInt("NormalDifficulty") == 1)
        {
            GetObject(GameObjects.NORMALINE).gameObject.SetActive(true);
            GetObject(GameObjects.HARDCORELINE).gameObject.SetActive(false);
        }
        else
        {
            GetObject(GameObjects.NORMALINE).gameObject.SetActive(true);
            GetObject(GameObjects.HARDCORELINE).gameObject.SetActive(false);
        }

        // check slider values
        Get<Slider>(Sliders.MusicSlider).GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume");
        Get<Slider>(Sliders.SensitivityXSlider).GetComponent<Slider>().value = PlayerPrefs.GetFloat("XSensitivity");
        Get<Slider>(Sliders.SensitivityYSlider).GetComponent<Slider>().value = PlayerPrefs.GetFloat("YSensitivity");
        Get<Slider>(Sliders.SmoothingSlider).GetComponent<Slider>().value = PlayerPrefs.GetFloat("MouseSmoothing");

        // check full screen
        if (Screen.fullScreen == true)
        {
            GetObject(GameObjects.fullscreentext).GetComponent<TMP_Text>().text = "on";
        }
        else if (Screen.fullScreen == false)
        {
            GetObject(GameObjects.fullscreentext).GetComponent<TMP_Text>().text = "off";
        }

        // check hud value
        if (PlayerPrefs.GetInt("ShowHUD") == 0)
        {
            GetObject(GameObjects.showhudtext).GetComponent<TMP_Text>().text = "off";
        }
        else
        {
            GetObject(GameObjects.showhudtext).GetComponent<TMP_Text>().text = "on";
        }

        // check tool tip value
        if (PlayerPrefs.GetInt("ToolTips") == 0)
        {
            GetObject(GameObjects.tooltiptext).GetComponent<TMP_Text>().text = "off";
        }
        else
        {
            GetObject(GameObjects.tooltiptext).GetComponent<TMP_Text>().text = "on";
        }

        if (PlayerPrefs.GetInt("Shadows") == 0)
        {
            QualitySettings.shadowCascades = 0;
            QualitySettings.shadowDistance = 0;
            GetObject(GameObjects.OFFLINE).gameObject.SetActive(true);
            GetObject(GameObjects.LOWLINE).gameObject.SetActive(false);
            GetObject(GameObjects.HIGHLINE).gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Shadows") == 1)
        {
            QualitySettings.shadowCascades = 2;
            QualitySettings.shadowDistance = 75;
            GetObject(GameObjects.OFFLINE).gameObject.SetActive(false);
            GetObject(GameObjects.LOWLINE).gameObject.SetActive(true);
            GetObject(GameObjects.HIGHLINE).gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Shadows") == 2)
        {
            QualitySettings.shadowCascades = 4;
            QualitySettings.shadowDistance = 500;
            GetObject(GameObjects.OFFLINE).gameObject.SetActive(false);
            GetObject(GameObjects.LOWLINE).gameObject.SetActive(false);
            GetObject(GameObjects.HIGHLINE).gameObject.SetActive(true);
        }

        // check vsync
        if (QualitySettings.vSyncCount == 0)
        {
            GetObject(GameObjects.vsynctext).GetComponent<TMP_Text>().text = "off";
        }
        else if (QualitySettings.vSyncCount == 1)
        {
            GetObject(GameObjects.vsynctext).GetComponent<TMP_Text>().text = "on";
        }

        // check mouse inverse
        if (PlayerPrefs.GetInt("Inverted") == 0)
        {
            GetObject(GameObjects.invertmousetext).GetComponent<TMP_Text>().text = "off";
        }
        else if (PlayerPrefs.GetInt("Inverted") == 1)
        {
            GetObject(GameObjects.invertmousetext).GetComponent<TMP_Text>().text = "on";
        }

        // check motion blur
        if (PlayerPrefs.GetInt("MotionBlur") == 0)
        {
            GetObject(GameObjects.motionblurtext).GetComponent<TMP_Text>().text = "off";
        }
        else if (PlayerPrefs.GetInt("MotionBlur") == 1)
        {
            GetObject(GameObjects.motionblurtext).GetComponent<TMP_Text>().text = "on";
        }

        // check ambient occlusion
        if (PlayerPrefs.GetInt("AmbientOcclusion") == 0)
        {
            GetObject(GameObjects.ambientocclusiontext).GetComponent<TMP_Text>().text = "off";
        }
        else if (PlayerPrefs.GetInt("AmbientOcclusion") == 1)
        {
            GetObject(GameObjects.ambientocclusiontext).GetComponent<TMP_Text>().text = "on";
        }

        // check texture quality
        if (PlayerPrefs.GetInt("Textures") == 0)
        {
            QualitySettings.globalTextureMipmapLimit = 2;
            GetObject(GameObjects.LOWLINE1).gameObject.SetActive(true);
            GetObject(GameObjects.MEDLINE1).gameObject.SetActive(false);
            GetObject(GameObjects.HIGHLINE1).gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Textures") == 1)
        {
            QualitySettings.globalTextureMipmapLimit = 1;
            GetObject(GameObjects.LOWLINE1).gameObject.SetActive(false);
            GetObject(GameObjects.MEDLINE1).gameObject.SetActive(true);
            GetObject(GameObjects.HIGHLINE1).gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetInt("Textures") == 2)
        {
            QualitySettings.globalTextureMipmapLimit = 0;
            GetObject(GameObjects.LOWLINE1).gameObject.SetActive(false);
            GetObject(GameObjects.MEDLINE1).gameObject.SetActive(false);
            GetObject(GameObjects.HIGHLINE1).gameObject.SetActive(true);
        }

        GetObject(GameObjects.PanelVideo).SetActive(false);
        GetObject(GameObjects.PanelControls).SetActive(false);
        GetObject(GameObjects.PanelKeyBindings).SetActive(false);
        GetObject(GameObjects.KeyConfirmation).SetActive(false);
        GetObject(GameObjects.PanelGame).SetActive(true);
        GetObject(GameObjects.LineControls).SetActive(false);
        GetObject(GameObjects.LineKeyBindings).SetActive(false);
        GetObject(GameObjects.LineVideo).SetActive(false);

        return true;
    }

    public void GamePanel()
    {
        DisablePanels();
        GetObject(GameObjects.PanelGame).SetActive(true);
        GetObject(GameObjects.LineGame).SetActive(true);
    }

    public void VideoPanel()
    {
        DisablePanels();
        GetObject(GameObjects.PanelVideo).SetActive(true);
        GetObject(GameObjects.LineVideo).SetActive(true);
    }

    public void ControlsPanel()
    {
        DisablePanels();
        GetObject(GameObjects.PanelControls).SetActive(true);
        GetObject(GameObjects.LineControls).SetActive(true);
    }

    public void KeyBindingsPanel()
    {
        DisablePanels();
        MovementPanel();
        GetObject(GameObjects.PanelKeyBindings).SetActive(true);
        GetObject(GameObjects.LineKeyBindings).SetActive(true);
    }

    public void MovementPanel()
    {
        DisablePanels();
        GetObject(GameObjects.PanelKeyBindings).SetActive(true);
        GetObject(GameObjects.MovementPanel).SetActive(true);
        GetObject(GameObjects.LineMovement).SetActive(true);
    }

    public void CombatPanel()
    {
        DisablePanels();
        GetObject(GameObjects.PanelKeyBindings).SetActive(true);
        GetObject(GameObjects.CombatPanel).SetActive(true);
        GetObject(GameObjects.LineCombat).SetActive(true);
    }

    public void GeneralPanel()
    {
        DisablePanels();
        GetObject(GameObjects.PanelKeyBindings).SetActive(true);
        GetObject(GameObjects.GeneralPanel).SetActive(true);
        GetObject(GameObjects.LineGeneral).SetActive(true);
    }

    void DisablePanels()
    {
        GetObject(GameObjects.PanelControls).SetActive(false);
        GetObject(GameObjects.PanelVideo).SetActive(false);
        GetObject(GameObjects.PanelGame).SetActive(false);
        GetObject(GameObjects.PanelKeyBindings).SetActive(false);

        GetObject(GameObjects.LineGame).SetActive(false);
        GetObject(GameObjects.LineControls).SetActive(false);
        GetObject(GameObjects.LineVideo).SetActive(false);
        GetObject(GameObjects.LineKeyBindings).SetActive(false);

        GetObject(GameObjects.MovementPanel).SetActive(false);
        GetObject(GameObjects.LineMovement).SetActive(false);
        GetObject(GameObjects.CombatPanel).SetActive(false);
        GetObject(GameObjects.LineCombat).SetActive(false);
        GetObject(GameObjects.GeneralPanel).SetActive(false);
        GetObject(GameObjects.LineGeneral).SetActive(false);
    }

    public void ExitMenu()
    {
        GetObject(GameObjects.PanelVideo).SetActive(false);
        GetObject(GameObjects.PanelControls).SetActive(false);
        GetObject(GameObjects.PanelGame).SetActive(false);
        GetObject(GameObjects.PanelKeyBindings).SetActive(false);
        GetObject(GameObjects.KeyConfirmation).SetActive(false);
        GetObject(GameObjects.LineControls).SetActive(false);
        GetObject(GameObjects.LineKeyBindings).SetActive(false);
        GetObject(GameObjects.LineVideo).SetActive(false);
    }

    public void Update()
        
    {
        //sliderValue = musicSlider.GetComponent<Slider>().value;
        _sliderValueXSensitivity = Get<Slider>(Sliders.SensitivityXSlider).GetComponent<Slider>().value;
        _sliderValueYSensitivity = Get<Slider>(Sliders.SensitivityYSlider).GetComponent<Slider>().value;
        _sliderValueSmoothing = Get<Slider>(Sliders.SmoothingSlider).GetComponent<Slider>().value;
    }

    public void FullScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;

        if (Screen.fullScreen == true)
        {
            GetObject(GameObjects.fullscreentext).GetComponent<TMP_Text>().text = "on";
        }
        else if (Screen.fullScreen == false)
        {
            GetObject(GameObjects.fullscreentext).GetComponent<TMP_Text>().text = "off";
        }
    }

    public void MusicSlider()
    {
        PlayerPrefs.SetFloat("MusicVolume", Get<Slider>(Sliders.MusicSlider).value * 2);
        Managers.SoundMng.UpdateVolume();
        Debug.Log($"Music Volume: {PlayerPrefs.GetFloat("MusicVolume")}");
    }

    public void SensitivityXSlider()
    {
        PlayerPrefs.SetFloat("XSensitivity", _sliderValueXSensitivity);
    }

    public void SensitivityYSlider()
    {
        PlayerPrefs.SetFloat("YSensitivity", _sliderValueYSensitivity);
    }

    public void SensitivitySmoothing()
    {
        PlayerPrefs.SetFloat("MouseSmoothing", _sliderValueSmoothing);
        Debug.Log(PlayerPrefs.GetFloat("MouseSmoothing"));
    }

    public void ShadowsOff()
    {
        PlayerPrefs.SetInt("Shadows", 0);
        QualitySettings.shadowCascades = 0;
        QualitySettings.shadowDistance = 0;
        GetObject(GameObjects.OFFLINE).SetActive(true);
        GetObject(GameObjects.LOWLINE).SetActive(false);
        GetObject(GameObjects.HIGHLINE).SetActive(false);
    }

    public void ShadowsLow()
    {
        PlayerPrefs.SetInt("Shadows", 1);
        QualitySettings.shadowCascades = 2;
        QualitySettings.shadowDistance = 75;
        GetObject(GameObjects.OFFLINE).SetActive(false);
        GetObject(GameObjects.LOWLINE).SetActive(true);
        GetObject(GameObjects.HIGHLINE).SetActive(false);
    }

    public void ShadowsHigh()
    {
        PlayerPrefs.SetInt("Shadows", 2);
        QualitySettings.shadowCascades = 4;
        QualitySettings.shadowDistance = 500;
        GetObject(GameObjects.OFFLINE).SetActive(false);
        GetObject(GameObjects.LOWLINE).SetActive(false);
        GetObject(GameObjects.HIGHLINE).SetActive(true);
    }

    public void vsync()
    {
        if (QualitySettings.vSyncCount == 0)
        {
            QualitySettings.vSyncCount = 1;
            GetObject(GameObjects.vsynctext).GetComponent<TMP_Text>().text = "on";
        }
        else if (QualitySettings.vSyncCount == 1)
        {
            QualitySettings.vSyncCount = 0;
            GetObject(GameObjects.vsynctext).GetComponent<TMP_Text>().text = "off";
        }
    }

    public void InvertMouse()
    {
        if (PlayerPrefs.GetInt("Inverted") == 0)
        {
            PlayerPrefs.SetInt("Inverted", 1);
            GetObject(GameObjects.invertmousetext).GetComponent<TMP_Text>().text = "on";
        }
        else if (PlayerPrefs.GetInt("Inverted") == 1)
        {
            PlayerPrefs.SetInt("Inverted", 0);
            GetObject(GameObjects.invertmousetext).GetComponent<TMP_Text>().text = "off";
        }
    }

    public void MotionBlur()
    {
        if (PlayerPrefs.GetInt("MotionBlur") == 0)
        {
            PlayerPrefs.SetInt("MotionBlur", 1);
            GetObject(GameObjects.motionblurtext).GetComponent<TMP_Text>().text = "on";
        }
        else if (PlayerPrefs.GetInt("MotionBlur") == 1)
        {
            PlayerPrefs.SetInt("MotionBlur", 0);
            GetObject(GameObjects.motionblurtext).GetComponent<TMP_Text>().text = "off";
        }
    }

    public void AmbientOcclusion()
    {
        if (PlayerPrefs.GetInt("AmbientOcclusion") == 0)
        {
            PlayerPrefs.SetInt("AmbientOcclusion", 1);
            GetObject(GameObjects.ambientocclusiontext).GetComponent<TMP_Text>().text = "on";
        }
        else if (PlayerPrefs.GetInt("AmbientOcclusion") == 1)
        {
            PlayerPrefs.SetInt("AmbientOcclusion", 0);
            GetObject(GameObjects.ambientocclusiontext).GetComponent<TMP_Text>().text = "off";
        }
    }

    public void TexturesLow()
    {
        PlayerPrefs.SetInt("Textures", 0);
        QualitySettings.globalTextureMipmapLimit = 2;
        GetObject(GameObjects.LOWLINE1).gameObject.SetActive(true);
        GetObject(GameObjects.MEDLINE1).gameObject.SetActive(false);
        GetObject(GameObjects.HIGHLINE1).gameObject.SetActive(false);
    }

    public void TexturesMed()
    {
        PlayerPrefs.SetInt("Textures", 1);
        QualitySettings.globalTextureMipmapLimit = 1;
        GetObject(GameObjects.LOWLINE1).gameObject.SetActive(false);
        GetObject(GameObjects.MEDLINE1).gameObject.SetActive(true);
        GetObject(GameObjects.HIGHLINE1).gameObject.SetActive(false);
    }

    public void TexturesHigh()
    {
        PlayerPrefs.SetInt("Textures", 2);
        QualitySettings.globalTextureMipmapLimit = 0;
        GetObject(GameObjects.LOWLINE1).gameObject.SetActive(false);
        GetObject(GameObjects.MEDLINE1).gameObject.SetActive(false);
        GetObject(GameObjects.HIGHLINE1).gameObject.SetActive(true);
    }
}

