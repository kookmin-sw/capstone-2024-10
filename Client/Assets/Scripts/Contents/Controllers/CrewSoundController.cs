using Fusion;
using UnityEngine;
using UnityEngine.ProBuilder;

public class CrewSoundController : BaseSoundController
{
    public float ChasingBgmVolume { get; protected set; }
    protected override void Init()
    {
        base.Init();

        ChasingDistance = 25f;
    }

    public override void PlayMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                Rpc_PlayFootStepSound(1.205f, 0.045f, 10f);
                break;
            case Define.CreaturePose.Sit:
                Rpc_StopEffectSound();
                break;
            case Define.CreaturePose.Run:
                Rpc_PlayFootStepSound(2f, 0.7f);
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
                PlayAdrenalineSound();
                break;
            case Define.CrewActionType.GameEnd:
                PlayEndSound();
                break;
            case Define.CrewActionType.Exhaust:
                PlayExhaust();
                break;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayFootStepSound(float pitch, float volume, float maxDistance = 20f)
    {
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Crew/FootStep", pitch, volume, isLoop:true, maxDistance: maxDistance);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayDamagedSound()
    {
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Crew/Damaged", pitch: 1f, volume: 0.3f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayDeadSound()
    {
        string effect = "GameOver";
        float volume = 0.5f;
        if (!HasStateAuthority)
        {
            effect = "GameOver2";
            volume = 0.5f;
        }

        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/Crew/{effect}", Define.SoundType.Facility, pitch: 1f, volume, isOneShot:true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayBandageSound()
    {
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Crew/Bandage", pitch: 1f, volume: 0.7f, isLoop: true);
    }

    protected void PlayAntipsychoticSound()
    {
        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/Crew/Antipsychotic", Define.SoundType.Effect,  volume:0.5f, isOneShot: true);
    }

    protected void PlayAdrenalineSound()
    {
        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/Crew/Adrenaline", Define.SoundType.Effect,  volume:0.35f, isOneShot: true);
    }

    protected void PlayExhaust()
    {
        Managers.SoundMng.Stop(Define.SoundType.Effect);
        Managers.SoundMng.Play($"{Define.EFFECT_PATH}/Crew/Exhaust", Define.SoundType.Effect, volume: 0.5f, isOneShot: true);
    }

    public override void CheckChasing()
    {
        if (!HasStateAuthority || !Creature.IsSpawned)
            return;

        IsChasing = false;

        Collider[] hitColliders = new Collider[1];
        if (Physics.OverlapBoxNonAlloc(CreatureCamera.Transform.position, new Vector3(ChasingDistance, 1f, ChasingDistance), hitColliders, Quaternion.identity, LayerMask.GetMask("Alien")) > 0)
        {
            if (hitColliders[0].gameObject.TryGetComponent(out Alien alien))
            {
                IsChasing = true;
                ChasingBgmVolume = Mathf.Clamp(-0.044f * (alien.Transform.position - Creature.Transform.position).magnitude + 1.4f, 0f, 1f) ;
            }
        }
    }

    public override void UpdateChasingValue()
    {
        if (IsChasing)
        {
            if (!Managers.SoundMng.IsPlaying(Define.SoundType.Bgm))
            {
                Managers.SoundMng.Play($"{Define.BGM_PATH}/Infernal Darkness", Define.SoundType.Bgm, volume: ChasingBgmVolume);
            }
            BgmAudioSource.volume = ChasingBgmVolume;
        }
        else
        {
            BgmAudioSource.volume -= 0.3f * Time.deltaTime;

            if (BgmAudioSource.volume <= 0f)
                Managers.SoundMng.Stop(Define.SoundType.Bgm);
        }
    }
}
