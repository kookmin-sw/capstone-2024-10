using Fusion;
using UnityEngine;

public class AlienSoundController : BaseSoundController
{
    public float ChasingValue { get; protected set; }

    protected override void Init()
    {
        base.Init();

        ChasingDistance = 20f;
    }

    public override void PlayMove()
    {
        Rpc_PlayFootStepSound(1.33f, 0.7f);
    }

    public void PlaySound(Define.AlienActionType alienActionType)
    {
        switch (alienActionType)
        {
            case Define.AlienActionType.GetBlind:
                Rpc_PlayDamaged();
                break;
            case Define.AlienActionType.Hit:
                Rpc_PlayHit();
                break;
            case Define.AlienActionType.BasicAttack:
                Rpc_PlayBasicAttack();
                break;
            case Define.AlienActionType.Roar:
                Rpc_PlayRoar();
                break;
            case Define.AlienActionType.CursedHowl:
                Rpc_PlayCursedHowl();
                break;
            case Define.AlienActionType.LeapAttack:
                Rpc_PlayLeapAttack();
                break;
            case Define.AlienActionType.GameEnd:
                PlayEndSound();
                break;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayFootStepSound(float pitch, float volume)
    {
        if (HasStateAuthority)
            volume *= 0.3f;

        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/FootStep_Alien", pitch, volume, isLoop: true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayDamaged()
    {
        float volume = 0.9f;
        if (HasStateAuthority)
            volume *= 0.5f;

        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Damaged_Alien", pitch: 1f, volume, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayBasicAttack()
    {
        float volume = 0.9f;
        if (HasStateAuthority)
            volume *= 0.5f;

        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Attack", pitch: 1f, volume, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayRoar()
    {
        float volume = 0.7f;
        if (HasStateAuthority)
            volume *= 0.5f;

        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Roar", pitch: 1f, volume, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayCursedHowl()
    {
        float volume = 0.7f;
        if (HasStateAuthority)
            volume *= 0.5f;

        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/CursedHowl", pitch: 1f, volume, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayLeapAttack()
    {
        float volume = 0.9f;
        if (HasStateAuthority)
            volume *= 0.5f;

        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Attack", pitch: 1f, volume, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayHit()
    {
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Attack_Hit", pitch: 1f, volume: 0.5f, isLoop: false);
    }

    public override void CheckChasing()
    {
        if (!HasStateAuthority || !Creature.IsSpawned)
            return;

        IsChasing = false;

        for (float i = 0.2f; i < 0.9f; i += 0.1f)
        {
            for (float j = 0.2f; j < 0.9f; j += 0.1f)
            {
                Ray ray = CreatureCamera.Camera.ViewportPointToRay(
                    new Vector3(i, j, CreatureCamera.Camera.nearClipPlane));

                if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: ChasingDistance, layerMask: LayerMask.GetMask("Crew", "MapObject", "InteractableObject")))
                {
                    if (rayHit.transform.gameObject.TryGetComponent(out Crew crew))
                    {
                        IsChasing = true;
                    }
                }

                // if (i < 0.25f || j < 0.25f || i > 0.75f || j > 0.75f)
                //     Debug.DrawRay(ray.origin, ray.direction * ChasingDistance, Color.green);
            }
        }
    }

    public override void UpdateChasingValue()
    {
        if (IsChasing)
            ChasingValue = Mathf.Clamp(ChasingValue + 100f * Time.deltaTime, 0f, 100f);
        else
            ChasingValue = Mathf.Clamp(ChasingValue - 15f * Time.deltaTime, 0f, 100f);

        if (ChasingValue >= 100f)
        {
            BgmAudioSource.volume = 0.4f;
            if (!Managers.SoundMng.IsPlaying(Define.SoundType.Bgm))
            {
                Managers.SoundMng.Play($"{Define.BGM_PATH}/In Captivity", Define.SoundType.Bgm, volume: 0.4f);
            }
        }
        else if (ChasingValue <= 0f)
        {
            BgmAudioSource.volume -= 0.3f * Time.deltaTime;

            if (BgmAudioSource.volume <= 0f)
                Managers.SoundMng.Stop(Define.SoundType.Bgm);
        }

        Debug.Log(ChasingValue);
    }
}
