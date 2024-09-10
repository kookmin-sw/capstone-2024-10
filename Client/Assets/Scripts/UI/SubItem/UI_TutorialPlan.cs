using TMPro;
using UnityEngine;

public class UI_TutorialPlan : UI_Base
{
    private GameObject planBatteryCharge;
    private GameObject planUseComputer;

    enum GameObjects
    {
        Plan_BatteryCharge,
        Plan_UseComputer,
    }

    enum Texts
    {
        Main_Title_text,
        Objective_Text,
    }

    public override bool Init()
    {
        if (base.Init() == false) return false;

        Bind<GameObject>(typeof(GameObjects));
        Bind<TMP_Text>(typeof(Texts));

        planBatteryCharge = GetObject(GameObjects.Plan_BatteryCharge);
        planUseComputer = GetObject(GameObjects.Plan_UseComputer);

        planBatteryCharge.SetActive(true);
        planUseComputer.SetActive(false);

        UpdateBatteryCount(0);

        return true;
    }

    public void UpdateBatteryCount(int batteryCount)
    {
        planBatteryCharge.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>()
            .SetText($"Insert Batteries In Battery Charger ({batteryCount}/2)", true);
        if(batteryCount >= 2)
        {
            planBatteryCharge.SetActive(false);
            planUseComputer.SetActive(true);

            planUseComputer.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>()
                .SetText($"Use Card Key on Central Control Computer", true);
        }
    }

    public void OnCardkeyUsed()
    {
        planUseComputer.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>()
            .SetText($"Use Central Control Computer", true);
    }

    public void OnCentralComputerUsed()
    {
        planUseComputer.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>()
            .SetText($"Use Cargo Gate Control Computer", true);
    }

    public void OnCargoGateComputerUsed()
    {
        planUseComputer.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>()
            .SetText($"Escape Through the Cargo Gate!", true);
    }
}
