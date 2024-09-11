using DG.Tweening;
using System;
using UnityEngine;

public class SettingSystem : MonoBehaviour
{
    #region Screen
    private bool _initialized = false;
    private int _quality = 3;
    private int _width = 1280;
    private int _height = 720;
    private int _fullScreen = 0;
    private int _vsync = 0;
    private int _screenRatioIndex = 0;
    private float _sensitivity = 1.0f;
    public int Width
    {
        get
        {
            Init();
            return _width;
        }
        private set
        {
            _width = value;
        }
    }
    public int Height
    {
        get
        {
            Init();
            return _height;
        }
        private set
        {
            _height = value;
        }
    }
    public int FullScreen
    {
        get
        {
            Init();
            return _fullScreen;
        }
        private set
        {
            _fullScreen = value;
        }
    }
    public int VSycn
    {
        get
        {
            Init();
            return _vsync;
        }
        private set
        {
            _vsync = value;
        }
    }
    public int ScreenRatioIndex
    {
        get
        {
            Init();
            return _screenRatioIndex;
        }
        private set
        {
            _screenRatioIndex = value;
        }
    }
    public int Quality
    {
        get
        {
            Init();
            return _quality;
        }
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
    public float Sensitivity
    {
        get
        {
            Init();
            return _sensitivity;
        }
        private set
        {
            _sensitivity = value;
        }
    }
    #endregion

    public void Init()
    {
        if (_initialized)
            return;

        Managers.GameMng.SettingSystem = this;

        Sensitivity = PlayerPrefs.GetFloat("Sensitivity", 1.0f);
        Width = PlayerPrefs.GetInt("ScreenWidth", 1280);
        Height = PlayerPrefs.GetInt("ScreenHeight", 720);
        Quality = PlayerPrefs.GetInt("Textures", 3);
        VSycn = PlayerPrefs.GetInt("VSync", 0);
        ScreenRatioIndex = PlayerPrefs.GetInt("ScreenRatio", 0);
        FullScreen = PlayerPrefs.GetInt("FullScreen", 0);

        _initialized = true;

        DOVirtual.DelayedCall(0.1f, () => {
            SetMusicVolume(Define.VolumeType.Bgm, GetMusicVolume(Define.VolumeType.Bgm));
            SetMusicVolume(Define.VolumeType.Effect, GetMusicVolume(Define.VolumeType.Effect));
            SetMusicVolume(Define.VolumeType.Environment, GetMusicVolume(Define.VolumeType.Environment));
            SetMusicVolume(Define.VolumeType.Master, GetMusicVolume(Define.VolumeType.Master));
            SetQuality(Quality);
            SelectResolution(ScreenRatioIndex);
            SetMouseSensitivity(Sensitivity);
            SetVSync(VSycn);
        });
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
        Managers.SoundMng.UpdateVolume();
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
        Screen.SetResolution(Width, Height, Convert.ToBoolean(FullScreen));
    }

    public void SetFullScreen(bool fullscreen)
    {
        FullScreen = fullscreen ? 1 : 0;
        Screen.SetResolution(Width, Height, fullscreen);
        PlayerPrefs.SetInt("FullScreen", FullScreen);
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
