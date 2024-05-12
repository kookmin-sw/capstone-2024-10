using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingSystem : MonoBehaviour
{
    public int Width = 1920;
    public int Height = 1080;
    public bool Fullscreen = true;
    public float XSensitivity = 1.0f;
    public float YSensitivity = 1.0f;
    private int _quality = 1;
    public int VSycn = 0;
    public int Quality
    {
        get { return _quality; }
        set
        {
            PlayerPrefs.SetInt("Textures", value);
            QualitySettings.SetQualityLevel(value);
            _quality = value;
        }
    }

    public void Init()
    {
        Managers.GameMng.SettingSystem = this;

        XSensitivity = PlayerPrefs.GetFloat("XSensitivity", 1.0f);
        YSensitivity = PlayerPrefs.GetFloat("YSensitivity", 1.0f);
        Width = PlayerPrefs.GetInt("ScreenWidth", 1920);
        Height = PlayerPrefs.GetInt("ScreenHeight", 1080);
        Quality = PlayerPrefs.GetInt("Textures", 1);
        VSycn = PlayerPrefs.GetInt("VSycn", 0);
    }

    public void SetResolution(int width, int height)
    {
        Width = width;
        Height = height;
        PlayerPrefs.SetFloat("ScreenWidth", width);
        PlayerPrefs.SetFloat("ScreenHeight", height);
        Screen.SetResolution(Width, Height, Fullscreen);
    }

    public void SetFullScreen(bool fullscreen)
    {
        Fullscreen = fullscreen;
        Screen.SetResolution(Width, Height, Fullscreen);
    }

    public void SetSensitivity(float sliderValueXSensitivity, float sliderValueYSensitivity)
    {
        XSensitivity = sliderValueXSensitivity;
        YSensitivity = sliderValueYSensitivity;
        PlayerPrefs.SetFloat("XSensitivity", sliderValueXSensitivity);
        PlayerPrefs.SetFloat("YSensitivity", sliderValueYSensitivity);
    }

    public void SetVSync(int vsync)
    {
        VSycn = vsync;
        PlayerPrefs.SetInt("VSycn", vsync);
        QualitySettings.vSyncCount = vsync;
    }
}
