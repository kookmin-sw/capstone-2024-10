using System;
using UnityEngine;

public class UI_CrewTutorial : UI_CrewIngame
{
    public UI_TutorialPlan TutorialPlanUI { get; private set; }

    enum SubItemUIsForTuto
    {
        UI_TutorialPlan,
    }

    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<UI_Base>(typeof(SubItemUIsForTuto));

        TutorialPlanUI = Get<UI_Base>(Enum.GetNames(typeof(SubItemUIsForTuto)).Length + (int)SubItemUIsForTuto.UI_TutorialPlan) as UI_TutorialPlan;
        PlanUI.gameObject.SetActive(false);
        TutorialPlanUI.gameObject.SetActive(true);

        return true;
    }

    public override void InitAfterNetworkSpawn(Creature creature)
    {
        base.InitAfterNetworkSpawn(creature);
    }
}
