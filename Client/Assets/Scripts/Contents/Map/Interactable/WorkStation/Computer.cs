using System.Collections;
using UnityEngine;

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
        WorkingDescription = "Fixing Computer";
        InteractDescription = "Fix computer";
    }

    public override void PlayInteractAnimation()
    {
        MyCrew.CrewAnimController.PlayKeypadUse();
    }
}
