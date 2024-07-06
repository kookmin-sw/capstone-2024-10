using Fusion;
using UnityEngine;

public class AlienSoundController : BaseSoundController
{
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
        if (HasStateAuthority)
            volume *= 0.5f;

        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/FootStep_Alien", pitch, volume, isLoop: true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayDamaged()
    {
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Damaged_Alien", pitch: 1f, volume: 1f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayCrashDoor()
    {
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/CrashDoor", pitch: 1f, volume: 1f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayBasicAttack()
    {
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Attack", pitch: 1f, volume: 1f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayRoar()
    {
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Roar", pitch: 1.3f, volume: 0.4f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayCursedHowl()
    {
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/CursedHowl", pitch: 1f, volume: 0.5f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayLeapAttack()
    {
        Managers.SoundMng.PlayObjectAudio(CreatureAudioSource, $"{Define.EFFECT_PATH}/Alien/Attack", pitch: 1f, volume: 1f, isLoop: false);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayHit()
    {
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
                     Debug.DrawRay(ray.origin, ray.direction * ChasingDistance, Color.green);

                if (Physics.Raycast(ray, out RaycastHit rayHit, maxDistance: ChasingDistance,
                        layerMask: LayerMask.GetMask("Crew", "MapObject", "InteractableObject")))
                {
                    if (rayHit.transform.gameObject.TryGetComponent(out Crew crew))
                    {
                        if (!IsChasing)
                        {
                            StopAllCoroutines();
                            IsChasing = true;

                            if (!Managers.SoundMng.IsPlaying(Define.SoundType.Bgm))
                            {
                                Managers.SoundMng.Play($"{Define.BGM_PATH}/In Captivity", Define.SoundType.Bgm,
                                    volume: 0.5f);
                            }
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
