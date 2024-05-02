using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    public static AudioSource[] _audioSources = new AudioSource[(int)Define.SoundType.MaxCount];
    public float[] _audioVolume = new float[(int)Define.SoundType.MaxCount];
    public Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    public void Init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.SoundType));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
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

    public void Play(string path, Define.SoundType type = Define.SoundType.Effect, float pitch = 1.0f, float volume = 1.0f, bool isLoop = false)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch, volume, isLoop);
    }

    public void Play(AudioClip audioClip, Define.SoundType type = Define.SoundType.Effect, float pitch = 1.0f, float volume = 1.0f, bool isLoop = false)
    {
        if (audioClip == null)
            return;

        AudioSource audioSource = null;
        float audioVolume = 1.0f;
        if (type == Define.SoundType.Effect)
        {
            audioSource = _audioSources[(int)Define.SoundType.Effect];
            audioVolume = _audioVolume[(int)Define.SoundType.Effect];
            audioSource.loop = isLoop;
        }
        else if (type == Define.SoundType.Bgm)
        {
            audioSource = _audioSources[(int)Define.SoundType.Bgm];
            audioVolume = _audioVolume[(int)Define.SoundType.Bgm];
        }
        else
        {
            audioSource = _audioSources[(int)Define.SoundType.Environment];
            audioVolume = _audioVolume[(int)Define.SoundType.Environment];
        }

        if (audioSource == null)
            return;

        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.pitch = pitch;
        audioSource.volume = volume * audioVolume;
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    public void PlayObjectAudio(AudioSource audioSource, string path, float pitch = 1.0f, float volume = 1.0f, bool isLoop = false)
    {
        if (audioSource == null)
            return;

        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.pitch = pitch;
        audioSource.volume = volume * _audioVolume[(int)Define.SoundType.Environment];
        audioSource.clip = Managers.SoundMng.GetOrAddAudioClip(path);

        if (audioSource.clip == null)
            return;

        audioSource.loop = isLoop;
        audioSource.Play();
    }

    public void Stop(Define.SoundType type = Define.SoundType.Effect)
    {
        AudioSource audioSource;
        if (type == Define.SoundType.Bgm)
        {
            audioSource = _audioSources[(int)Define.SoundType.Bgm];
        }
        else if (type == Define.SoundType.Environment)
        {
            audioSource = _audioSources[(int)Define.SoundType.Environment];
        }
        else
        {
            audioSource = _audioSources[(int)Define.SoundType.Effect];
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

        if (type == Define.SoundType.Bgm || type == Define.SoundType.Environment)
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
        AudioSource audioSource;
        if (type == Define.SoundType.Bgm)
            audioSource = _audioSources[(int)Define.SoundType.Bgm];
        else if(type == Define.SoundType.Environment)
            audioSource = _audioSources[(int)Define.SoundType.Environment];
        else
            audioSource = _audioSources[(int)Define.SoundType.Effect];

        if (audioSource == null)
            return false;

        return audioSource.isPlaying;
    }

    public void UpdateVolume()
    {
        // TODO: 종류 별로 볼륨을 다르게 설정할 수 있도록 수정
        for (int i = 0; i < _audioVolume.Length; i++)
        {
            _audioVolume[i] = PlayerPrefs.GetFloat("MusicVolume", 1f);
            // _audioVolume[i] = 1f; // TODO - Test code
            _audioSources[i].volume = _audioVolume[i];
        }
    }
}
