using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

public class SoundManager
{
    public static AudioSource[] _audioSources = new AudioSource[(int)Define.SoundType.MaxCount];
    private static AudioMixerGroup[] _audioMixerGroups;
    public static Dictionary<Define.SoundType, AudioMixerGroup> AudioMixerGroups = new();
    public static Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    public static AudioMixer Mixer;

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            UnityEngine.Object.DontDestroyOnLoad(root);

            Mixer = Managers.ResourceMng.Load<AudioMixer>(Define.AUDIO_MIXER_PATH);

            if (Mixer == null)
                return;

            _audioMixerGroups = Mixer.FindMatchingGroups("Master");

            for (int i = 0; i < (int)Define.SoundType.MaxCount; i++)
            {
                for (int j = 0; j < _audioMixerGroups.Length; j++)
                {
                    var type = (Define.SoundType)i;
                    if (GetVolumeType(type).ToString() == _audioMixerGroups[j].name)
                    {
                        AudioMixerGroups.Add(type, _audioMixerGroups[j]);
                        break;
                    }
                }
            }

            string[] soundNames = System.Enum.GetNames(typeof(Define.SoundType));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                var type = (Define.SoundType)i;
                
                if (AudioMixerGroups.TryGetValue(type, out var mixer))
                {
                    _audioSources[i].outputAudioMixerGroup = mixer;
                }
                else
                {
                    Debug.Log("Can't find outputAudioMixerGroup");
                }
                
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.SoundType.Bgm].loop = true;
            _audioSources[(int)Define.SoundType.Environment].loop = true;
        }

        UpdateVolume();
    }

    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
        _audioClips.Clear();
    }

    public void Play(string path, Define.SoundType type = Define.SoundType.Effect, float pitch = 1.0f, float volume = 1.0f, bool isLoop = false, bool isOneShot = false)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch, volume, isLoop, isOneShot);
    }

    public void Play(AudioClip audioClip, Define.SoundType type = Define.SoundType.Effect, float pitch = 1.0f, float volume = 1.0f, bool isLoop = false, bool isOneShot = false)
    {
        if (audioClip == null)
            return;

        AudioSource audioSource = null;
        if (type == Define.SoundType.Effect)
        {
            audioSource = _audioSources[(int)Define.SoundType.Effect];
            audioSource.loop = isLoop;
        }
        else if (type == Define.SoundType.Bgm)
        {
            audioSource = _audioSources[(int)Define.SoundType.Bgm];
        }
        else if (type == Define.SoundType.Environment)
        {
            audioSource = _audioSources[(int)Define.SoundType.Environment];
        }
        else if (type == Define.SoundType.Facility)
        {
            audioSource = _audioSources[(int)Define.SoundType.Facility];
            audioSource.loop = isLoop;
        }

        if (audioSource == null)
            return;

        if (isOneShot)
        {
            audioSource.PlayOneShot(audioClip, volume);
            return;
        }

        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void PlayObjectAudio(AudioSource audioSource, string path,
                                float pitch = 1.0f, float volume = 1.0f, bool isLoop = false, Define.SoundType soundType = Define.SoundType.Facility)
    {
        if (audioSource == null)
            return;

        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.pitch = pitch;
        audioSource.volume = volume;
        audioSource.clip = Managers.SoundMng.GetOrAddAudioClip(path);

        if (audioSource.clip == null)
            return;

        audioSource.loop = isLoop;
        audioSource.outputAudioMixerGroup = AudioMixerGroups[soundType];
        audioSource.Play();
    }

    public void Stop(Define.SoundType type = Define.SoundType.Effect)
    {
        AudioSource audioSource = null;
        if (type == Define.SoundType.Bgm)
        {
            audioSource = _audioSources[(int)Define.SoundType.Bgm];
        }
        else if (type == Define.SoundType.Environment)
        {
            audioSource = _audioSources[(int)Define.SoundType.Environment];
        }
        else if (type == Define.SoundType.Effect)
        {
            audioSource = _audioSources[(int)Define.SoundType.Effect];
        }
        else if (type == Define.SoundType.Facility)
        {
            audioSource = _audioSources[(int)Define.SoundType.Facility];
        }

        if (audioSource == null)
            return;

        audioSource.Stop();
    }

    public AudioClip GetOrAddAudioClip(string path, Define.SoundType type = Define.SoundType.Effect)
    {
        if (path.Contains("Sounds/") == false)
            path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == Define.SoundType.Environment || type == Define.SoundType.Facility)
        {
            audioClip = Managers.ResourceMng.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.ResourceMng.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing!: {path}");

        return audioClip;
    }

    public bool IsPlaying(Define.SoundType type = Define.SoundType.Effect)
    {
        AudioSource audioSource = null;
        if (type == Define.SoundType.Bgm)
            audioSource = _audioSources[(int)Define.SoundType.Bgm];
        else if(type == Define.SoundType.Environment)
            audioSource = _audioSources[(int)Define.SoundType.Environment];
        else if (type == Define.SoundType.Effect)
            audioSource = _audioSources[(int)Define.SoundType.Effect];
        else if (type == Define.SoundType.Facility)
            audioSource = _audioSources[(int)Define.SoundType.Facility];

        if (audioSource == null)
            return false;

        return audioSource.isPlaying;
    }

    public Define.VolumeType GetVolumeType(Define.SoundType soundType)
    {
        int volumeType = -1;

        switch (soundType)
        {
            case Define.SoundType.Bgm:
                volumeType = (int)Define.VolumeType.Bgm;
                break;
            case Define.SoundType.Environment:
                volumeType = (int)Define.VolumeType.Environment;
                break;
            case Define.SoundType.Effect:
                volumeType = (int)Define.VolumeType.Effect;
                break;
            case Define.SoundType.Facility:
                volumeType = (int)Define.VolumeType.Effect;
                break;
        }

        Assert.IsTrue(volumeType != -1);

        return (Define.VolumeType) volumeType;
    }

    public void MuteOn()
    {
        var volumeType = Define.VolumeType.Master;
        float volume = Mathf.Log10(0.0001f) * 20;
        Mixer.SetFloat(volumeType.ToString(), volume);
    }

    public void MuteOff()
    {
        var volumeType = Define.VolumeType.Master;
        float volume = Mathf.Log10(PlayerPrefs.GetFloat(volumeType.ToString(), 1f)) * 20;
        Mixer.SetFloat(volumeType.ToString(), volume);
    }

    public void UpdateVolume()
    {
        for (int i = 0; i < (int)Define.VolumeType.MaxCount; i++)
        {
            var volumeType = (Define.VolumeType)i;
            float volume = Mathf.Log10(PlayerPrefs.GetFloat(volumeType.ToString(), 1f)) * 20;
            Mixer.SetFloat(volumeType.ToString(), volume);
        }
    }
}
