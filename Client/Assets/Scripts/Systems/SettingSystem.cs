using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingSystem : MonoBehaviour
{
    #region Screen
    public int Width { get; set; } = 1920;
    public int Height { get; set; } = 1080;
    public bool Fullscreen { get; set; } = true;
    private int _quality = 1;
    public int VSycn = 0;
    public int Quality
    {
        get { return _quality; }
        set
        {
            PlayerPrefs.SetInt("Textures", value);
            SetQuality(value);
            _quality = value;
        }
    }
    #endregion

    #region Input
    public float Sensitivity = 1.0f;
    #endregion

    public void Init()
    {
        Managers.GameMng.SettingSystem = this;

        Sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        Width = PlayerPrefs.GetInt("ScreenWidth", 1920);
        Height = PlayerPrefs.GetInt("ScreenHeight", 1080);
        Quality = PlayerPrefs.GetInt("Textures", 1);
        VSycn = PlayerPrefs.GetInt("VSync", 0);
    }

    private void SetQuality(int index)
    {
        QualitySettings.globalTextureMipmapLimit = 3 - index;
    }

    public void SetMusicVolume(Define.VolumeType volumeType, float value)
    {
        string field = volumeType.ToString();
        PlayerPrefs.SetFloat(field, value);
        Debug.Log($"{field}: {PlayerPrefs.GetFloat(field)}");
    }

    public void SetResolution(int width, int height)
    {
        Width = width;
        Height = height;
        PlayerPrefs.SetInt("ScreenWidth", width);
        PlayerPrefs.SetInt("ScreenHeight", height);
        Screen.SetResolution(Width, Height, Fullscreen);
    }

    public void SetFullScreen(bool fullscreen)
    {
        Fullscreen = fullscreen;
        Screen.SetResolution(Width, Height, Fullscreen);
    }

    public void SetMouseSensitivity(float sliderValueSensitivity)
    {
        Sensitivity = sliderValueSensitivity;
        PlayerPrefs.SetFloat("Sensitivity", sliderValueSensitivity);
    }

    public void SetVSync(int vsync)
    {
        VSycn = vsync;
        PlayerPrefs.SetInt("VSync", vsync);
        QualitySettings.vSyncCount = vsync;
    }
}
