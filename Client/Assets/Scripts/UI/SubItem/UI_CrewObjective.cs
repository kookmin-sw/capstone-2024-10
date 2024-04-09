using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrewObjective : UI_Base
{
    enum GameObjects
    {
        PlanA,
        PlanB,
        PlanC
    }

    enum Texts
    {
        PlanA_Objective_Text,
        PlanB_Objective_Text,
        PlanC_Objective_Text,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TMP_Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        GetObject(GameObjects.PlanC).SetActive(false);
        return true;
    }

    public void UpdateBatteryCount(int count)
    {
        GetText(Texts.PlanA_Objective_Text).text = $"a. Collect and charge the batteries ({count}/{Define.BATTERY_COLLECT_GOAL})";
        if (count >= Define.BATTERY_COLLECT_GOAL)
        {
            GetText(Texts.PlanA_Objective_Text).text =
                $"a. Collect and charge the batteries ({Define.BATTERY_COLLECT_GOAL}/{Define.BATTERY_COLLECT_GOAL})";
            GetText(Texts.PlanA_Objective_Text).DOColor(Color.green, 0.3f).OnComplete((() =>
            {
                GetText(Texts.PlanA_Objective_Text).text = "b. Restore the backup generator (0/1)";
            }));
        }
    }

    public void OnGeneratorRestored()
    {
        
    }
}

