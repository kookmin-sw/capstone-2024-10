using System;
using UnityEngine;

public class SettingSystem : MonoBehaviour
{
    #region Screen
    public int Width { get; set; } = 1280;
    public int Height { get; set; } = 720;
    public int Fullscreen { get; set; } = 0;
    private int _quality = 3;
    public int VSycn = 0;
    public int ScreenRatioIndex = 0;
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

    private int[,] _screenResolution = new int[,]
    {
        {1280, 720},
        {1920, 1080},
        {2560, 1440},
    };
    #endregion

    #region Input
    public float Sensitivity = 1.0f;
    #endregion

    public void Init()
    {
        Managers.GameMng.SettingSystem = this;

        Sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        Width = PlayerPrefs.GetInt("ScreenWidth", 1280);
        Height = PlayerPrefs.GetInt("ScreenHeight", 720);
        Quality = PlayerPrefs.GetInt("Textures", 3);
        VSycn = PlayerPrefs.GetInt("VSync", 0);
        ScreenRatioIndex = PlayerPrefs.GetInt("ScreenRatio", 0);
        Fullscreen = PlayerPrefs.GetInt("FullScreen", 0);
    }

    private void SetQuality(int index)
    {
        QualitySettings.globalTextureMipmapLimit = 3 - index;
    }

    public float GetMusicVolume(Define.VolumeType volumeType)
    {
        string field = volumeType.ToString();
        return PlayerPrefs.GetFloat(field, 1.0f);
    }

    public void SetMusicVolume(Define.VolumeType volumeType, float value)
    {
        string field = volumeType.ToString();
        PlayerPrefs.SetFloat(field, value);
        Debug.Log($"{field}: {PlayerPrefs.GetFloat(field)}");
    }

    public void SelectResolution(int idx)
    {
        int width = _screenResolution[idx, 0];
        int height = _screenResolution[idx, 1];
        Debug.Log($"{width} x {height}");
        SetResolution(width, height);
        ScreenRatioIndex = idx;
        PlayerPrefs.SetInt("ScreenRatio", idx);
    }

    public void SetResolution(int width, int height)
    {
        Width = width;
        Height = height;
        PlayerPrefs.SetInt("ScreenWidth", width);
        PlayerPrefs.SetInt("ScreenHeight", height);
        Screen.SetResolution(Width, Height, Convert.ToBoolean(Fullscreen));
    }

    public void SetFullScreen(bool fullscreen)
    {
        Fullscreen = fullscreen ? 1 : 0;
        Screen.SetResolution(Width, Height, fullscreen);
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
