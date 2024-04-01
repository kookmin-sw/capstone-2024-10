public class Computer : BaseWorkStation
{
    public Crew MyCrew => (Crew)MyWorker;

    protected override void Init()
    {
        base.Init();

        InteractDescription = "USE COMPUTER";

        CanUseAgain = false;
        CanRememberWork = true;
        CanCollaborate = true;
        IsCompleted = false;

        TotalWorkAmount = 100f;
    }

    public override void PlayInteract()
    {
        MyCrew.CrewAnimController.PlayKeypadUse();
    }
}
