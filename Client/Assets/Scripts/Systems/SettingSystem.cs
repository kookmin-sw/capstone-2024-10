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
    public int Quality
    {
        get { return _quality; }
        set
        {
            PlayerPrefs.SetInt("Textures", 1);
            QualitySettings.SetQualityLevel(1);
            _quality = value;
        }
    }

    enum Options
    {

    }

    public void Init()
    {
        Managers.GameMng.SettingSystem = this;

        XSensitivity = PlayerPrefs.GetFloat("XSensitivity", 1.0f);
        YSensitivity = PlayerPrefs.GetFloat("YSensitivity", 1.0f);
    }

    void Update()
    {
        if (Quality != PlayerPrefs.GetInt("Textures"))
        {
            QualitySettings.SetQualityLevel(Quality);
        }

        Screen.SetResolution(Width, Height, Fullscreen);
    }

    public void SetResolution(int width, int height)
    {
        Width = width;
        Height = height;
        Screen.SetResolution(Width, Height, Fullscreen);
    }

    public void SetFullScreen(bool fullscreen)
    {
        Fullscreen = fullscreen;
        Screen.SetResolution(Width, Height, Fullscreen);
    }

    public void SetSensitivity(float sliderValueXSensitivity, float sliderValueYSensitivity)
    {
        Debug.Log("Set Sensitivity");
        XSensitivity = sliderValueXSensitivity;
        YSensitivity = sliderValueYSensitivity;
        PlayerPrefs.SetFloat("XSensitivity", sliderValueXSensitivity);
        PlayerPrefs.SetFloat("YSensitivity", sliderValueYSensitivity);
    }
}
