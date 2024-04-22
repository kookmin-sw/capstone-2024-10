using Fusion;

public class CrewSoundController : BaseSoundController
{
    public override void PlayMove()
    {
        switch (CreaturePose)
        {
            case Define.CreaturePose.Stand:
                Rpc_PlayFootStepSound(1f, 0.3f);
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
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Crew/Walk");
        AudioSource.pitch = pitch;
        AudioSource.volume = volume;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.loop = true;
        AudioSource.Play();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayDamagedSound()
    {
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Crew/Damaged");
        AudioSource.volume = 0.5f;
        AudioSource.pitch = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.loop = false;
        AudioSource.Play();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayDeadSound()
    {
        AudioSource.clip = Managers.SoundMng.GetOrAddAudioClip($"{Define.EFFECT_PATH}/Crew/Damaged");
        AudioSource.volume = 0.5f;
        AudioSource.pitch = 1f;
        AudioSource.spatialBlend = 1.0f;
        AudioSource.loop = false;
        AudioSource.Play();
    }
}
