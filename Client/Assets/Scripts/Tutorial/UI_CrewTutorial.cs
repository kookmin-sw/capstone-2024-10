using System;
using UnityEngine;

public class UI_CrewTutorial : UI_CrewIngame
{
    public GameObject TutorialPlanUI { get; private set; }

    enum SubItemUIsForTuto
    {
        UI_TutorialPlan,
    }

    public override bool Init()
    {
        if (base.Init() == false) { return false; }

        Bind<GameObject>(typeof(SubItemUIsForTuto));

        TutorialPlanUI = GetObject(SubItemUIsForTuto.UI_TutorialPlan);
        PlanUI.gameObject.SetActive(false);
        TutorialPlanUI.gameObject.SetActive(true);

        CrewStaminaUI.gameObject.SetActive(false);
        CrewHpUI.gameObject.SetActive(false);
        CurrentSectorUI.gameObject.SetActive(false);

        return true;
    }

    public override void InitAfterNetworkSpawn(Creature creature)
    {
        // base.InitAfterNetworkSpawn(creature);
    }
}
