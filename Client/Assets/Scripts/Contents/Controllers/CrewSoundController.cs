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
            case Define.CrewActionType.Bandage:
                Rpc_PlayBandageSound();
                break;
            case Define.CrewActionType.Antipsychotic:
                Rpc_PlayAntipsychoticSound();
                break;
            case Define.CrewActionType.Morphine:
                Rpc_PlayMorphineSound();
                break;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayFootStepSound(float pitch, float volume)
    {
        CreatureAudioSource.spatialBlend = 1.0f;
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Crew/FootStep", pitch, volume, isLoop: true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayDamagedSound()
    {
        CreatureAudioSource.loop = false;
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Crew/Damaged", pitch: 1f, volume: 0.5f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.StateAuthority)]
    protected void Rpc_PlayDeadSound()
    {
        CreatureAudioSource.spatialBlend = 1.0f;
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Crew/GameOver", pitch: 1f, volume: 1f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayBandageSound()
    {
        CreatureAudioSource.spatialBlend = 1.0f;
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Crew/Bandage", pitch: 1f, volume: 1f, isLoop: true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayAntipsychoticSound()
    {
        CreatureAudioSource.spatialBlend = 1.0f;
        Managers.SoundMng.PlayOneShotObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Crew/Antipsychotic", volume: 1f);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayMorphineSound()
    {
        CreatureAudioSource.spatialBlend = 1.0f;
        Managers.SoundMng.PlayOneShotObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Crew/Morphine", volume: 1f);
    }

    public override void CheckChasing()
    {
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
