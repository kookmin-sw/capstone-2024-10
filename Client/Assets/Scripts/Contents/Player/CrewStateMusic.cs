using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Fusion;
using UnityEngine;

public class CrewStateMusic : NetworkBehaviour
{

    public AudioSource AudioSource { get; set; }
    public Crew Crew { get; set; }
    // Start is called before the first frame update


    public override void Spawned()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public void CheckHurtMusic()
    {
        if (AudioSource.isPlaying == false)
        {
            Rpc_PlayHurtMusic();
        }
        else
        {
            return;
        }
    }
    public void StopHurtMusic()
    {
        if (AudioSource.isPlaying == true)
        {
            Rpc_StopEffectMusic();
        }
        else
        {
            return;
        }
    }


    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_PlayHurtMusic()
    {
        AudioSource.volume = 0.5f;
        AudioSource.pitch = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip("Music/Clicks/Hurt");
        AudioSource.loop = true;
        AudioSource.Play();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_StopEffectMusic()
    {
        AudioSource.Stop();
    }
}
