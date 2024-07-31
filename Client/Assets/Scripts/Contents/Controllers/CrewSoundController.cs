using Fusion;
using UnityEngine;

public class CrewSoundController : BaseSoundController
{
    protected override void Init()
    {
        base.Init();

        ChasingDistance = 20f;
    }

    public override void PlayMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                Rpc_PlayFootStepSound(1.205f, 0.15f);
                break;
            case Define.CreaturePose.Sit:
                Rpc_StopEffectSound();
                break;
            case Define.CreaturePose.Run:
                Rpc_PlayFootStepSound(2f, 0.9f);
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
                PlayAntipsychoticSound();
                break;
            case Define.CrewActionType.Adrenaline:
                PlayMorphineSound();
                break;
            case Define.CrewActionType.GameEnd:
                PlayEndSound();
                break;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayFootStepSound(float pitch, float volume)
    {
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Crew/FootStep", pitch, volume, isLoop:true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayDamagedSound()
    {
        CreatureAudioSource.loop = false;
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Crew/Damaged", pitch: 1f, volume: 0.5f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayDeadSound()
    {
        string effect = "GameOver";
        float volume = 0.7f;
        if (!HasStateAuthority)
        {
            effect = "GameOver2";
            volume *= 0.85f;
        }

        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/Crew/{effect}", Define.SoundType.Effect, pitch: 1f, volume);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayBandageSound()
    {
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Crew/Bandage", pitch: 1f, volume: 1f, isLoop: true);
    }

    protected void PlayAntipsychoticSound()
    {
        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/Crew/Antipsychotic", Define.SoundType.Effect,  volume:0.5f, isOneShot: true);
    }

    protected void PlayMorphineSound()
    {
        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/Crew/Morphine", Define.SoundType.Effect,  volume:0.35f, isOneShot: true);
    }

    public override void CheckChasing()
    {
        Collider[] hitColliders = new Collider[1];
        if (Physics.OverlapBoxNonAlloc(CreatureCamera.Transform.position, new Vector3(ChasingDistance, 1f, ChasingDistance),
                hitColliders, Quaternion.identity, LayerMask.GetMask("Alien")) > 0)
        {
            if (hitColliders[0].gameObject.TryGetComponent(out Alien alien))
            {
                if (!IsChasing)
                {
                    StopAllCoroutines();
                    IsChasing = true;

                    if (!Managers.SoundMng.IsPlaying(Define.SoundType.Bgm))
                        Managers.SoundMng.Play($"{Define.BGM_PATH}/Infernal Darkness", Define.SoundType.Bgm, volume: 0.2f);
                }

                SoundManager._audioSources[(int)Define.SoundType.Bgm].volume =
                    -0.05f * (alien.Transform.position - Creature.Transform.position).magnitude + 1.2f;
                return;
            }
        }

        if (IsChasing)
            StartCoroutine(CheckNotChasing(2f));

        IsChasing = false;
    }
}
