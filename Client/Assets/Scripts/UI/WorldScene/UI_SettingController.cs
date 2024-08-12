using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

public class UI_SettingController : UI_Base
{
    #region Enums
    enum GameObjects
    {
        fullscreentext,
        vsynctext,
        LOWLINE1,
        MEDLINE1,
        HIGHLINE1,
        PanelControls,
        PanelVideo,
        PanelGame,
        LineGame,
        LineVideo,
        LineControls,
    }

    enum Buttons
    {
        Btn_Return,
        Btn_Game,
        Btn_Controls,
        Btn_Video,
        FullScreen_Btn,
        VSync_Btn,
        TextureLow_Btn,
        TextureMed_Btn,
        TextureHigh_Btn,
    }

    enum Dropdowns
    {
        Dropdown,
    }

    enum Sliders
    {
        SensitivitySlider,
        MusicSlider,
        EnvMusicSlider,
        EffMusicSlider,
        BgmMusicSlider,
    }

    #endregion

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

        Get<Slider>(Sliders.MusicSlider).gameObject.BindEvent((e) => { MusicSlider(Define.VolumeType.MasterVolume, Sliders.MusicSlider); }, Define.UIEvent.Drag);
        Get<Slider>(Sliders.BgmMusicSlider).gameObject.BindEvent((e) => { MusicSlider(Define.VolumeType.BgmVolume, Sliders.BgmMusicSlider); }, Define.UIEvent.Drag);
        Get<Slider>(Sliders.EffMusicSlider).gameObject.BindEvent((e) => { MusicSlider(Define.VolumeType.EffVolume, Sliders.EffMusicSlider); }, Define.UIEvent.Drag);
        Get<Slider>(Sliders.EnvMusicSlider).gameObject.BindEvent((e) => { MusicSlider(Define.VolumeType.EnvVolume, Sliders.EnvMusicSlider); }, Define.UIEvent.Drag);
        Get<Slider>(Sliders.MusicSlider).gameObject.BindEvent((e) => { MusicSlider(Define.VolumeType.MasterVolume, Sliders.MusicSlider); });
        Get<Slider>(Sliders.BgmMusicSlider).gameObject.BindEvent((e) => { MusicSlider(Define.VolumeType.BgmVolume, Sliders.BgmMusicSlider); });
        Get<Slider>(Sliders.EffMusicSlider).gameObject.BindEvent((e) => { MusicSlider(Define.VolumeType.EffVolume, Sliders.EffMusicSlider); });
        Get<Slider>(Sliders.EnvMusicSlider).gameObject.BindEvent((e) => { MusicSlider(Define.VolumeType.EnvVolume, Sliders.EnvMusicSlider); });

        Get<Slider>(Sliders.SensitivitySlider).gameObject.BindEvent((e) => { SensitivitySlider(); }, Define.UIEvent.Drag);
        GetButton(Buttons.FullScreen_Btn).gameObject.BindEvent((e) => { FullScreen(); }, Define.UIEvent.Click);
        GetButton(Buttons.VSync_Btn).gameObject.BindEvent((e) => { vsync(); }, Define.UIEvent.Click);
        GetButton(Buttons.TextureLow_Btn).gameObject.BindEvent((e) => { TexturesLow(); }, Define.UIEvent.Click);
        GetButton(Buttons.TextureMed_Btn).gameObject.BindEvent((e) => { TexturesMed(); }, Define.UIEvent.Click);
        GetButton(Buttons.TextureHigh_Btn).gameObject.BindEvent((e) => { TexturesHigh(); }, Define.UIEvent.Click);

        // check slider values
        Get<Slider>(Sliders.BgmMusicSlider).value = PlayerPrefs.GetFloat(Define.VolumeType.BgmVolume.ToString());
        Get<Slider>(Sliders.EffMusicSlider).value = PlayerPrefs.GetFloat(Define.VolumeType.EffVolume.ToString());
        Get<Slider>(Sliders.EnvMusicSlider).value = PlayerPrefs.GetFloat(Define.VolumeType.EnvVolume.ToString());
        Get<Slider>(Sliders.MusicSlider).value = PlayerPrefs.GetFloat(Define.VolumeType.MasterVolume.ToString());

        Get<Slider>(Sliders.SensitivitySlider).value = Managers.GameMng.SettingSystem.Sensitivity;


        var dropdown = Get<TMP_Dropdown>(Dropdowns.Dropdown);
        dropdown.onValueChanged.AddListener((int value) =>
        {
            SelectResolution(dropdown.options[value].text);
        });

        // check full screen
        if (Screen.fullScreen == true)
        {
            GetObject(GameObjects.fullscreentext).GetComponent<TMP_Text>().text = "On";
        }
        else if (Screen.fullScreen == false)
        {
            GetObject(GameObjects.fullscreentext).GetComponent<TMP_Text>().text = "Off";
        }

        // check vsync
        if (Managers.GameMng.SettingSystem.VSycn == 0)
        {
            GetObject(GameObjects.vsynctext).GetComponent<TMP_Text>().text = "Off";
        }
        else if (Managers.GameMng.SettingSystem.VSycn == 1)
        {
            GetObject(GameObjects.vsynctext).GetComponent<TMP_Text>().text = "On";
        }

        // check texture quality
        if (Managers.GameMng.SettingSystem.Quality == 1)
        {
            TexturesLow();
        }
        else if (Managers.GameMng.SettingSystem.Quality == 2)
        {
            TexturesMed();
        }
        else if (Managers.GameMng.SettingSystem.Quality == 3)
        {
            TexturesHigh();
        }

        GetObject(GameObjects.PanelVideo).SetActive(false);
        GetObject(GameObjects.PanelControls).SetActive(false);
        GetObject(GameObjects.PanelGame).SetActive(true);
        GetObject(GameObjects.LineControls).SetActive(false);
        GetObject(GameObjects.LineVideo).SetActive(false);

        return true;
    }

    public void SelectResolution(string optionText)
    {
        if (optionText == "Select")
            return;

        string[] words = optionText.Split(' ');
        int width = int.Parse(words[0]);
        int height = int.Parse(words[2]);
        Debug.Log($"{width} x {height}");
        Managers.GameMng.SettingSystem.SetResolution(width, height);
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

    void DisablePanels()
    {
        GetObject(GameObjects.PanelControls).SetActive(false);
        GetObject(GameObjects.PanelVideo).SetActive(false);
        GetObject(GameObjects.PanelGame).SetActive(false);

        GetObject(GameObjects.LineGame).SetActive(false);
        GetObject(GameObjects.LineControls).SetActive(false);
        GetObject(GameObjects.LineVideo).SetActive(false);
    }

    public void ExitMenu()
    {
        GetObject(GameObjects.PanelVideo).SetActive(false);
        GetObject(GameObjects.PanelControls).SetActive(false);
        GetObject(GameObjects.PanelGame).SetActive(false);
        GetObject(GameObjects.LineControls).SetActive(false);
        GetObject(GameObjects.LineVideo).SetActive(false);
        GetObject(GameObjects.LineGame).SetActive(false);
    }

    public void FullScreen()
    {
        if (Screen.fullScreen == true)
        {
            GetObject(GameObjects.fullscreentext).GetComponent<TMP_Text>().text = "Off";
            Managers.GameMng.SettingSystem.SetFullScreen(false);
        }
        else if (Screen.fullScreen == false)
        {
            GetObject(GameObjects.fullscreentext).GetComponent<TMP_Text>().text = "On";
            Managers.GameMng.SettingSystem.SetFullScreen(true);
        }
    }

    private void MusicSlider(Define.VolumeType volumeType, Sliders sliderType)
    {
        string field = volumeType.ToString();
        PlayerPrefs.SetFloat(field, Get<Slider>(sliderType).value);
        Managers.SoundMng.UpdateVolume();
        Debug.Log($"{field}: {PlayerPrefs.GetFloat(field)}");
    }

    public void SensitivitySlider()
    {
        Managers.GameMng.SettingSystem.SetMouseSensitivity(Get<Slider>(Sliders.SensitivitySlider).value);
    }

    public void vsync()
    {
        if (Managers.GameMng.SettingSystem.VSycn == 0)
        {
            Managers.GameMng.SettingSystem.SetVSync(1);
            GetObject(GameObjects.vsynctext).GetComponent<TMP_Text>().text = "On";
        }
        else if (Managers.GameMng.SettingSystem.VSycn == 1)
        {
            Managers.GameMng.SettingSystem.SetVSync(0);
            GetObject(GameObjects.vsynctext).GetComponent<TMP_Text>().text = "Off";
        }
    }

    public void TexturesLow()
    {
        Managers.GameMng.SettingSystem.Quality = 1;
        GetObject(GameObjects.LOWLINE1).gameObject.SetActive(true);
        GetObject(GameObjects.MEDLINE1).gameObject.SetActive(false);
        GetObject(GameObjects.HIGHLINE1).gameObject.SetActive(false);
    }

    public void TexturesMed()
    {
        Managers.GameMng.SettingSystem.Quality = 2;
        GetObject(GameObjects.LOWLINE1).gameObject.SetActive(false);
        GetObject(GameObjects.MEDLINE1).gameObject.SetActive(true);
        GetObject(GameObjects.HIGHLINE1).gameObject.SetActive(false);
    }

    public void TexturesHigh()
    {
        Managers.GameMng.SettingSystem.Quality = 3;
        GetObject(GameObjects.LOWLINE1).gameObject.SetActive(false);
        GetObject(GameObjects.MEDLINE1).gameObject.SetActive(false);
        GetObject(GameObjects.HIGHLINE1).gameObject.SetActive(true);
    }
}

