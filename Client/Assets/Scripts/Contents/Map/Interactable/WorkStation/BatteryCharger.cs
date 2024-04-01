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

    public override bool IsInteractable(Creature creature, bool isDoInteract)
    {
        if (creature.CreatureType == Define.CreatureType.Alien)
            return false;

        if (!CanUseAgain && IsCompleted)
            return false;

        creature.IngameUI.InteractInfoUI.Show(InteractDescription.ToString());

        if (CurrentWorkers.Count >= 3 || (!CanCollaborate && CurrentWorkers.Count >= 1))
            return false;

        if (!(creature.CreatureState == Define.CreatureState.Idle || creature.CreatureState == Define.CreatureState.Move))
            return false;

        if (((Crew)creature).Inventory.CurrentItem.ItemType != Define.ItemType.Battery)
            return false;

        if (isDoInteract)
            Interact(creature);

        return true;
    }

    public override void WorkComplete()
    {
        MyCrew.Inventory.CurrentItemIdx = -1;

        base.WorkComplete();
    }


    public override void PlayInteractAnimation()
    {
        MyCrew.CrewAnimController.PlayKeypadUse();
    }
}

