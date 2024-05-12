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

        //TotalWorkAmount = 150f;
        TotalWorkAmount = 15f; // TODO: for test
    }

    public override bool IsInteractable(Creature creature)
    {
        if (!base.IsInteractable(creature)) return false;

        if (creature is not Crew)
        {
            return false;
        }

        if (!Managers.GameMng.PlanSystem.IsBatteryChargeFinished || Managers.GameMng.PlanSystem.IsPanicRoomActivated)
        {
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

        Debug.Log(panicRooms.Count);
        foreach (var room in panicRooms)
        {
            room.GetComponent<PanicRoom>().Rpc_ChangeLightColor();
        }
        PanicRoom unlockedPanicRoom = panicRooms[Random.Range(0, panicRooms.Count)].GetComponent<PanicRoom>();
        panicRooms.Remove(unlockedPanicRoom.gameObject);
        PanicRoom unlockedPanicRoom2 = panicRooms[Random.Range(0, panicRooms.Count)].GetComponent<PanicRoom>();

        Debug.Log("Unlocked :" + unlockedPanicRoom.name);
        Debug.Log("Unlocked :" + unlockedPanicRoom2.name);

        unlockedPanicRoom.IsLocked = false;
        unlockedPanicRoom2.IsLocked = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/GeneratorController", 1f, 1f, isLoop: true);
    }
}
