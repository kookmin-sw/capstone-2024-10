using Fusion;
using UnityEngine;

public class CrewSoundController : BaseSoundController
{
    public override void PlayMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                Rpc_PlayFootStepSound(1.205f, 0.2f);
                break;
            case Define.CreaturePose.Sit:
                Rpc_StopEffectMusic();
                break;
            case Define.CreaturePose.Run:
                Rpc_PlayFootStepSound(2f, 1f);
                break;
        }
    }

    public void PlaySound(Define.CrewActionType crewActionType)
    {
        switch (crewActionType)
        {
            case Define.CrewActionType.Damaged:
                Rpc_PlayDamagedSound();
                break;
            case Define.CrewActionType.Dead:
                Rpc_PlayDeadSound();
                break;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayFootStepSound(float pitch, float volume)
    {
        CreatureAudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Crew/FootStep");
        CreatureAudioSource.pitch = pitch;
        CreatureAudioSource.volume = volume;
        CreatureAudioSource.spatialBlend = 1.0f;
        CreatureAudioSource.loop = true;
        CreatureAudioSource.Play();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayDamagedSound()
    {
        CreatureAudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Crew/Damaged");
        CreatureAudioSource.volume = 0.5f;
        CreatureAudioSource.pitch = 1f;
        CreatureAudioSource.spatialBlend = 1.0f;
        CreatureAudioSource.loop = false;
        CreatureAudioSource.Play();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayDeadSound()
    {
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Crew/Dead");
        AudioSource.volume = 1f;
        AudioSource.pitch = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.loop = false;
        AudioSource.Play();
    }

    public override void CheckChasing()
    {
        if (!HasStateAuthority || !Creature.IsSpawned)
            return;

        Collider[] hitColliders = new Collider[1];
        if (Physics.OverlapBoxNonAlloc(CreatureCamera.Transform.position, new Vector3(25f, 1f, 25f),
                hitColliders, Quaternion.identity, LayerMask.GetMask("Alien")) > 0)
        {
            if (hitColliders[0].gameObject.TryGetComponent(out Alien alien))
            {

                SoundManager._audioSources[(int)Define.SoundType.Bgm].volume =
                    -0.045f * (alien.Transform.position - Creature.Transform.position).magnitude + 1.225f;
                if (!IsChasing)
                {
                    StopAllCoroutines();
                    IsChasing = true;

                    if (!Managers.SoundMng.IsPlaying(Define.SoundType.Bgm))
                        Managers.SoundMng.Play($"{Define.BGM_PATH}/Deep Space Pulsing Signals", Define.SoundType.Bgm, volume: 0.3f);
                }
                return;
            }
        }

        if (IsChasing)
            StartCoroutine(CheckNotChasing(2f));

        IsChasing = false;
    }
}
