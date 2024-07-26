using System.Collections;
using Fusion;
using UnityEngine;

public abstract class BaseSoundController : NetworkBehaviour
{
    public Creature Creature { get; protected set; }
    public CreatureCamera CreatureCamera => Creature.CreatureCamera;
    public AudioSource CreatureAudioSource => Creature.AudioSource;
    public Define.CreatureState CreatureState => Creature.CreatureState;
    public Define.CreaturePose CreaturePose => Creature.CreaturePose;

    public AudioSource BgmAudioSource => SoundManager._audioSources[(int)Define.SoundType.Bgm];

    public bool IsChasing { get; protected set; } = false;
    public float ChasingDistance { get; protected set; }

    public override void Spawned()
    {
        Init();
    }

    protected virtual void Init()
    {
        Creature = gameObject.GetComponent<Creature>();
    }

    public abstract void PlayMove();

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StopEffectSound()
    {
        if (CreatureAudioSource.isPlaying)
            CreatureAudioSource.Stop();
    }

    public void StopAllSound()
    {
        Managers.SoundMng.Stop(Define.SoundType.Bgm);
        Managers.SoundMng.Stop(Define.SoundType.Environment);
        Managers.SoundMng.Stop(Define.SoundType.Effect);
        Managers.SoundMng.Stop(Define.SoundType.Facility);

        Rpc_StopEffectSound();
    }

    #region Chasing

    public abstract void CheckChasing();

    protected IEnumerator CheckNotChasing(float time)
    {
        float currentChasingTime = 0f;
        while (currentChasingTime < time)
        {
            currentChasingTime += Time.deltaTime;
            yield return null;
        }

        StartCoroutine(StopChasing());
    }

    protected IEnumerator StopChasing()
    {
        while (BgmAudioSource.volume > 0f)
        {
            BgmAudioSource.volume -= 0.3f * Time.deltaTime;
            yield return null;
        }

        Managers.SoundMng.Stop(Define.SoundType.Bgm);
    }

    #endregion

    #region EndGame

    public void PlayEndGame(bool isDead = false)
    {
        if (isDead)
            Managers.SoundMng.Play($"{Define.EFFECT_PATH}/Crew/GameOver", Define.SoundType.Effect,  volume:0.7f);
        else
            Managers.SoundMng.Play($"{Define.BGM_PATH}/Panic Man", Define.SoundType.Bgm, volume: 0.7f);
    }

    #endregion
}
