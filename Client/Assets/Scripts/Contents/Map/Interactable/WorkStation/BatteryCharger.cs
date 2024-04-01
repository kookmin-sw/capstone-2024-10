using Fusion;
using UnityEngine;

public class BatteryCharger : BaseWorkStation
{
    public Crew MyCrew => (Crew)MyWorker;

    protected override void Init()
    {
        base.Init();

        InteractDescription = "USE COMPUTER";

        CanUseAgain = true;
        CanRememberWork = true;
        CanCollaborate = true;
        IsCompleted = false;

        TotalWorkAmount = 10f;
    }

    public override bool CheckAndInteract(Creature creature)
    {
        if (creature.CreatureType == Define.CreatureType.Alien)
            return false;

        if (!CanUseAgain && IsCompleted)
            return false;

        if (CurrentWorkers.Count >= 3 || (!CanCollaborate && CurrentWorkers.Count >= 1))
            return false;

        if (!(creature.CreatureState == Define.CreatureState.Idle || creature.CreatureState == Define.CreatureState.Move))
            return false;

        if (((Crew)creature).Inventory.CurrentItem.ItemType != Define.ItemType.Battery)
            return false;

        MyWorker = creature;
        MyWorker.IngameUI.InteractInfoUI.Hide();
        MyWorker.CreatureState = Define.CreatureState.Interact;
        MyWorker.CreaturePose = Define.CreaturePose.Stand;
        MyWorker.CurrentWorkStation = this;
        MyWorker.IngameUI.WorkProgressBarUI.Show(InteractDescription.ToString(), TotalWorkAmount);

        Rpc_AddWorker(MyWorker.NetworkObject.Id);
        PlayInteract();

        StartCoroutine(CoWorkProgress());

        return true;
    }

    public override void WorkComplete()
    {
        MyCrew.Inventory.CurrentItemIdx = -1;

        base.WorkComplete();
    }


    public override void PlayInteract()
    {
        MyCrew.CrewAnimController.PlayKeypadUse();
    }
}

