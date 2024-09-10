using Fusion;
using UnityEngine;

public class FlashBangObject : BaseItemObject
{
    public override int DataId => Define.ITEM_FLASHBANG_ID;
    public AudioSource AudioSource { get; protected set; }

    public GameObject ExplosionParticle { get; protected set; }

    public override void FixedUpdateNetwork()
    {
        ExplosionParticle.transform.rotation = Quaternion.identity;
    }

    public override void Init()
    {
        AudioSource = GetComponent<AudioSource>();
        ExplosionParticle = Util.FindChild(gameObject, "Explosion");
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_Explode()
    {
        ExplosionParticle.SetActive(true);
        gameObject.SetLayerRecursive(LayerMask.NameToLayer("MapObject"));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/FlashBang", soundType : Define.SoundType.Effect, volume: 0.3f);
    }
}
