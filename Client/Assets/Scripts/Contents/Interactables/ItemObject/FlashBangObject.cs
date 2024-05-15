using Fusion;
using UnityEngine;

public class FlashBangObject : BaseItemObject
{
    public override int DataId => Define.ITEM_FLASHBANG_ID;
    public AudioSource AudioSource { get; protected set; }

    public override void Init()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_PlaySound()
    {
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Interactable/FlashBang");
        AudioSource.volume = 0.5f;
        AudioSource.loop = false;
        AudioSource.Play();
    }
}
