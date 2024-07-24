using Fusion;
using UnityEngine;

public class ElevatorKeypad : BaseWorkStation
{
    [SerializeField] private GameObject _elevatorDoor;

    protected override void Init()
    {
        base.Init();

        Description = "Activate Elevator";
        CrewActionType = Define.CrewActionType.KeypadUse;
        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = true;
        IsCompleted = false;

        TotalWorkAmount = 10f;
    }
    public override bool IsInteractable(Creature creature)
    {
        if(!base.IsInteractable(creature)) return false;

        if (creature is not Crew)
        {
            return false;
        }

        if (IsCompleted)
        {
            creature.IngameUI.ErrorTextUI.Show("Already Used");
            return false;
        }

        if (!Managers.GameMng.PlanSystem.IsUSBKeyInsertFinished)
        {
            return false;
        }

        if (WorkerCount > 0 && Worker == null)
        {
            creature.IngameUI.ErrorTextUI.Show("Another Crew is in Use");
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        _elevatorDoor.GetComponent<NetworkMecanimAnimator>().Animator.SetBool("OpenParameter", true);
        Rpc_SetLayer();
        Rpc_PlayCompleteSound();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void Rpc_SetLayer()
    {
        gameObject.SetLayerRecursive(LayerMask.NameToLayer("MapObject"));
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/KeypadUse", 1f, 1f, isLoop: true);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    protected void Rpc_PlayCompleteSound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/SFDoor_Open", 1f, 1f, isLoop: false);
    }
}
