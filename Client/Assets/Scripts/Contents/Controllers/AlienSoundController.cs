using Fusion;

public class AlienSoundController : BaseSoundController
{
    public override void PlayMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                Rpc_PlayFootStepSound(1.33f, 0.3f);
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
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Alien/FootStep_Alien");
        AudioSource.pitch = pitch;
        AudioSource.volume = volume;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.loop = true;
        AudioSource.Play();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayCrashDoor()
    {
        AudioSource.pitch = 1f;
        AudioSource.volume = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.loop = false;
        AudioSource.PlayOneShot(Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Alien/CrashDoor"));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayBasicAttack()
    {
        AudioSource.pitch = 1f;
        AudioSource.volume = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.loop = false;
        AudioSource.PlayOneShot(Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Alien/Attack"));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayRoar()
    {
        AudioSource.pitch = 1.3f;
        AudioSource.volume = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.loop = false;
        AudioSource.PlayOneShot(Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Alien/Roar"));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayCursedHowl()
    {
        AudioSource.pitch = 1f;
        AudioSource.volume = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.loop = false;
        AudioSource.PlayOneShot(Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Alien/CursedHowl"));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayLeapAttack()
    {
        AudioSource.pitch = 1f;
        AudioSource.volume = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.loop = false;
        AudioSource.PlayOneShot(Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Alien/Attack"));
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayHit()
    {
        AudioSource.pitch = 1f;
        AudioSource.volume = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.loop = false;
        AudioSource.PlayOneShot(Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Alien/Attack_Hit"));
    }
}
