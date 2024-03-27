public class Computer : BaseWorkStation
{
    public Crew MyCrew => (Crew)MyWorker;

    protected override void Init()
    {
        base.Init();

        CanUseAgain = false;
        CanRememberWork = true;
        CanCollaborate = true;
        IsCompleted = false;

        TotalWorkAmount = 100f;
        Description = "COMPUTER";
    }

    public override void PlayInteract()
    {
        MyCrew.CrewAnimController.PlayKeypadUse();
    }
}
