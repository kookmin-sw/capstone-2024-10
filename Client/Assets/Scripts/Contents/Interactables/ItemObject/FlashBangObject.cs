using Fusion;
using UnityEngine;

public class FlashBangObject : BaseItemObject
{
    public AudioSource AudioSource { get; protected set; }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public override void Rpc_SetInfo(NetworkBool canGet)
    {
        DataId = Define.ITEM_FLASHBANG_ID;

        base.Rpc_SetInfo(canGet);

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
