using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class AlienTracking : NetworkBehaviour
{
    public AudioSource AudioSource { get; set; }
    // Start is called before the first frame update

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
        Rpc_PlayEffectMusic();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_PlayEffectMusic()
    {
        AudioSource.volume = 1f;
        AudioSource.pitch = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip("Music/Bgm/검은 숲의 추격자");
        AudioSource.loop = true;
        AudioSource.Play();
    }

}
