using Fusion;
using UnityEngine;

public class BatteryCharger : BaseWorkStation
{
    protected override void Init()
    {
        base.Init();

        Description = "Charge Battery";
        CrewActionType = Define.CrewActionType.ChargeBattery;
        AudioSource = gameObject.GetComponent<AudioSource>();
        CanRememberWork = false;
        IsCompleted = false;

        TotalWorkAmount = 1.8f;
    }

    public override bool IsInteractable(Creature creature)
    {
        if(!base.IsInteractable(creature)) return false;

        if (creature is not Crew crew)
        {
            return false;
        }

        if (Managers.GameMng.PlanSystem.IsBatteryChargeFinished)
        {
            creature.IngameUI.ErrorTextUI.Show("Battery Charge is Already Completed");
            return false;
        }

        // WorkerCount는 NetworkProperty이기 때문에 Master Client가 아닌 다른 Client가 작업을 도중에 취소하면, Rpc_RemoveWorker()에서 WorkerCount 값이 변경되어 동기화 되는 데 시간이 걸림.
        // 이 때 밑의 조건에서 WorkerCount > 0만 사용한다면 작업을 취소한 Client에게 아래의 에러 메시지가 잠깐동안 보이는 문제가 발생함.
        // 로컬 필드인 Worker를 사용해서 이를 해결함. InterruptWork()에서 0.5초간의 딜레이 후 Worker를 null로 초기화함. 또한, Worker != null이라면 상호작용이 불가능하도록 하여 버그를 방지함.
        if (WorkerCount > 0 && Worker == null)
        {
            creature.IngameUI.ErrorTextUI.Show("Another Crew is in Use");
            return false;
        }

        if (crew.Inventory.CurrentItem is not Battery)
        {
            creature.IngameUI.ErrorTextUI.Show("Hold Battery on Your Hand");
            return false;
        }

        creature.IngameUI.InteractInfoUI.Show(Description);
        return true;
    }

    protected override void WorkComplete()
    {
        CrewWorker.Inventory.RemoveItem();
        base.WorkComplete();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    protected override void Rpc_WorkComplete()
    {
        CurrentWorkAmount = 0;
        Managers.GameMng.PlanSystem.BatteryChargeCount++;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    protected override void Rpc_PlaySound()
    {
        Managers.SoundMng.PlayObjectAudio(AudioSource, $"{Define.EFFECT_PATH}/Interactable/BatteryCharger", 1f, 1f, isLoop: false);
    }
}

