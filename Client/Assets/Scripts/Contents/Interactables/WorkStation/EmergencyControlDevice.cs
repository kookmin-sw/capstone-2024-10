using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class EmergencyControlDevice : BaseWorkStation
{
    protected override void Init()
    {
        base.Init();

        Description ="Activate Panic Room";
        CrewActionType = Define.CrewActionType.KeypadUse;
        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = true;
        IsCompleted = false;

        TotalWorkAmount = 2f;
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

        if (creature is not Crew)
        {
            return false;
        }

        if (!Managers.GameMng.PlanSystem.IsBatteryChargeFinished)
        {
            return false;
        }

        if (Managers.GameMng.PlanSystem.IsPanicRoomActivated)
        {
            creature.IngameUI.ErrorTextUI.Show("Already Used");
            return false;
        }

        if (Managers.GameMng.GameEndSystem.CrewNum != 1)
        {
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

        Managers.GameMng.PlanSystem.IsPanicRoomActivated = true;
        List<GameObject> panicRooms = GameObject.FindGameObjectsWithTag("PanicRoom").ToList();

        foreach (var room in panicRooms)
        {
            room.GetComponent<PanicRoom>().Rpc_ChangeLightColor();
        }

        for (int i = 0; i < Define.OPEN_PANIC_ROOM; i++)
        {
            PanicRoom unlockedPanicRoom = panicRooms[Random.Range(0, panicRooms.Count)].GetComponent<PanicRoom>();
            panicRooms.Remove(unlockedPanicRoom.gameObject);
            Debug.Log("Unlocked :" + unlockedPanicRoom.name);
            unlockedPanicRoom.IsLocked = false;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/KeypadUse", 1f, 1f, isLoop: true);
    }
}
