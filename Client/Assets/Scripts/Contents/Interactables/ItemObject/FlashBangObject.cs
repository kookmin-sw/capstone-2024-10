using Fusion;
using UnityEngine;

public class FlashBangObject : BaseItemObject
{
    public override int DataId => Define.ITEM_FLASHBANG_ID;
    public AudioSource AudioSource { get; protected set; }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_SetInfo(NetworkBool isGettable)
    {
        base.Rpc_SetInfo(isGettable);

        AudioSource = GetComponent<AudioSource>();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_PlaySound()
    {
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Interactable/FlashBang");
        AudioSource.volume = 1f;
        AudioSource.loop = false;
        AudioSource.Play();
    }
}
