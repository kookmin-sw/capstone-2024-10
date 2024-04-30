using Fusion;
using UnityEngine;

public class AlienSoundController : BaseSoundController
{
    public override void PlayMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                Rpc_PlayFootStepSound(1.33f, 1f);
                break;
            case Define.CreaturePose.Run:
                Rpc_PlayFootStepSound(2f, 1f);
                break;
        }
    }

    public void PlaySound(Define.AlienActionType alienActionType)
    {
        switch (alienActionType)
        {
            case Define.AlienActionType.CrashDoor:
                Rpc_PlayCrashDoor();
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
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayFootStepSound(float pitch, float volume)
    {
        CreatureAudioSource.spatialBlend = 1.0f;
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/FootStep_Alien", pitch, volume, isLoop: true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayCrashDoor()
    {
        CreatureAudioSource.spatialBlend = 1.0f;
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/CrashDoor", pitch: 1f, volume: 1f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayBasicAttack()
    {
        CreatureAudioSource.spatialBlend = 1.0f;
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Attack", pitch: 1f, volume: 1f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayRoar()
    {
        CreatureAudioSource.spatialBlend = 1.0f;
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Roar", pitch: 1.3f, volume: 1f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayCursedHowl()
    {
        CreatureAudioSource.spatialBlend = 1.0f;
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/CursedHowl", pitch: 1f, volume: 1f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayLeapAttack()
    {
        CreatureAudioSource.spatialBlend = 1.0f;
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Attack", pitch: 1f, volume: 1f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayHit()
    {
        CreatureAudioSource.spatialBlend = 1.0f;
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Attack_Hit", pitch: 1f, volume: 1f, isLoop: false);
    }

    public override void CheckChasing()
    {
        if (!HasStateAuthority || !Creature.IsSpawned)
            return;

        for (float i = 0.2f; i < 0.9f; i += 0.1f)
        {
            for (float j = 0.2f; j < 0.9f; j += 0.1f)
            {
                Ray ray = CreatureCamera.Camera.ViewportPointToRay(
                    new Vector3(i, j, CreatureCamera.Camera.nearClipPlane));

                if (i < 0.25f || j < 0.25f || i > 0.75f || j > 0.75f)
                    Debug.DrawRay(ray.origin, ray.direction * 15f, Color.green);

                if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: 15f,
                        layerMask: LayerMask.GetMask("Crew", "MapObject")))
                {
                    if (rayHit.transform.gameObject.TryGetComponent(out Crew crew))
                    {
                        if (!IsChasing)
                        {
                            StopAllCoroutines();
                            IsChasing = true;
                            if (!Managers.SoundMng.IsPlaying(Define.SoundType.Bgm))
                                Managers.SoundMng.Play($"{Define.BGM_PATH}/The Big Clash", Define.SoundType.Bgm, volume: 1);
                        }

                        return;
                    }
                }
            }
        }

        if (IsChasing)
            StartCoroutine(CheckNotChasing(7f));

        IsChasing = false;
    }
}
