using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_TutorialPlan : UI_Base
{
    private GameObject planBatteryCharge;

    enum GameObjects
    {
        Plan_BatteryCharge,
    }

    enum Texts
    {
        Main_Title_text,
    }

    public override bool Init()
    {
        if (base.Init() == false) return false;

        Bind<GameObject>(typeof(GameObjects));
        Bind<TMP_Text>(typeof(Texts));

        planBatteryCharge = GetObject(GameObjects.Plan_BatteryCharge);

        return true;
    }

}
