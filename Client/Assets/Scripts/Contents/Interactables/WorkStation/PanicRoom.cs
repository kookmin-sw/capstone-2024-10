using Fusion;
using UnityEngine;

public class PanicRoom : BaseWorkStation
{
    [Networked] public NetworkBool IsLocked { get; set; } = true;

    [SerializeField] private Gate _gate;
    [SerializeField] private Light _entranceLight;

    protected override void Init()
    {
        base.Init();

        _gate = GetComponentInChildren<Gate>();

        Description = "Open panic room";
        CrewActionType = Define.CrewActionType.OpenDoor;
        CanRememberWork = false;

        TotalWorkAmount = 1f;
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

        if (creature is not Crew)
        {
            return false;
        }

        if (!Managers.GameMng.PlanSystem.IsPanicRoomActivated)
        {
            return false;
        }

        if (IsLocked)
        {
            creature.IngameUI.ErrorTextUI.Show("This panic room is not open!");
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    public override bool Interact(Creature creature)
    {
        Worker = creature;
        Worker.IngameUI.InteractInfoUI.Hide();
        Worker.IngameUI.ObjectNameUI.Hide();
        Worker.CreatureState = Define.CreatureState.Interact;
        Worker.CreaturePose = Define.CreaturePose.Stand;

        Rpc_AddWorker();
        PlayAnim();

        WorkComplete();
        InterruptWork();

        return true;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        if (IsCompleted) return;
        IsCompleted = true;
        Rpc_ChangeLightColor();
        Rpc_DisableCollider();
    }

    protected override void Rpc_PlaySound()
    {

    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void Rpc_ChangeLightColor()
    {
        _entranceLight.color = Color.green;
        float emissiveIntensity = 5;
        Color emissiveColor = Color.green;
        _entranceLight.GetComponentInParent<MeshRenderer>().material.SetColor("_EmissiveColor", emissiveColor * emissiveIntensity);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    private void Rpc_DisableCollider()
    {
        GetComponent<Collider>().enabled = false;

        _gate.Open();
        _gate.GetComponent<AudioSource>().Play();
    }
}
