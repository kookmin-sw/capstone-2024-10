using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    /// <summary>
    /// 오디오 소스를 담는 배열이다. Define에 정의되어 있는 MaxCount의 수만큼 사운드의 종류를 정의한다.
    /// </summary>
    AudioSource[] _audioSources = new AudioSource[(int)Define.Sound.MaxCount];
    /// <summary>
    /// 오디오 클립을 담는 딕셔너리로, 이름을 통해 딕셔너리에 있는 오디오 클립을 가져올 수 있다.
    /// </summary>
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

    /// <summary>
    /// 씬에 @Sound 오브젝트를 생성하고 파괴 불가 설정 후, 오디오 소스들을 등록해 놓는다.
    /// </summary>
    public void init()
    {
        GameObject root = GameObject.Find("@Sound");
        if (root == null)
        {
            root = new GameObject { name = "@Sound" };
            Object.DontDestroyOnLoad(root);

            string[] soundNames = System.Enum.GetNames(typeof(Define.Sound));
            for (int i = 0; i < soundNames.Length - 1; i++)
            {
                GameObject go = new GameObject { name = soundNames[i] };
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Define.Sound.Bgm].loop = true;
        }
    }

    /// <summary>
    /// 씬이 바뀔 때, 음악을 멈추고 오디오소스들을 삭제한다.
    /// </summary>
    public void Clear()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            audioSource.clip = null;
            audioSource.Stop();
        }
        _audioClips.Clear();
    }

    /// <summary>
    /// Resources/Music/ 폴더에 위치한 음원 이름을 입력하고 재생하고자 하는 type을 선택해서 재생한다.
    /// 음원은 type마다 병렬로 재생되며, 각각 처리되는 방식에 차이를 주고 있다.
    /// </summary>
    /// <param name="path">음원의 이름</param>
    /// <param name="type">음원의 타입</param>
    /// <param name="pitch"></param>
    public void Play(string path, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        AudioClip audioClip = GetOrAddAudioClip(path, type);
        Play(audioClip, type, pitch);
    }

    /// <summary>
    /// 호출하려면 음원파일을 로드하고 사용해야 한다.
    /// 음원 타입에 따라 어떤식으로 재생될지에 대한 방식이 지정되어 있다.
    /// </summary>
    public void Play(AudioClip audioClip, Define.Sound type = Define.Sound.Effect, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        if (type == Define.Sound.Bgm)
        {
            AudioSource audioSource = _audioSources[(int)Define.Sound.Bgm];

            if (audioSource.isPlaying)
                audioSource.Stop();

            //audioSource.pitch = pitch;
            audioSource.volume = pitch;
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {            
            AudioSource audioSource = _audioSources[(int)Define.Sound.Effect];
            //audioSource.pitch = pitch;
            audioSource.volume = pitch;
            audioSource.PlayOneShot(audioClip);
        }
    }

    /// <summary>
    /// 음원의 이름을 줬을 때, 음원을 딕셔너리에 저장하고 음원 파일을 가져오는 함수
    /// </summary>
    AudioClip GetOrAddAudioClip(string path, Define.Sound type = Define.Sound.Effect)
    {
        //if (path.Contains("Sounds/") == false)
        //  path = $"Sounds/{path}";

        AudioClip audioClip = null;

        if (type == Define.Sound.Bgm)
        {
            audioClip = Managers.Resource.Load<AudioClip>(path);
        }
        else
        {
            if (_audioClips.TryGetValue(path, out audioClip) == false)
            {
                audioClip = Managers.Resource.Load<AudioClip>(path);
                _audioClips.Add(path, audioClip);
            }
        }

        if (audioClip == null)
            Debug.Log($"AudioClip Missing ! {path}");

        return audioClip;
    }
}
