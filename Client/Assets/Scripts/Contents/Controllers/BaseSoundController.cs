using Fusion;
using UnityEngine;

public abstract class BaseSoundController : NetworkBehaviour
{
    public Creature Creature { get; protected set; }
    public AudioSource AudioSource => Creature.AudioSource;
    public Define.CreatureState CreatureState => Creature.CreatureState;
    public Define.CreaturePose CreaturePose => Creature.CreaturePose;

    public override void Spawned()
    {
        Init();
    }

    protected void Init()
    {
        Creature = gameObject.GetComponent<Creature>();
    }

    public abstract void PlayMove();

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StopEffectMusic()
    {
        if (AudioSource.isPlaying)
            AudioSource.Stop();
    }
}
