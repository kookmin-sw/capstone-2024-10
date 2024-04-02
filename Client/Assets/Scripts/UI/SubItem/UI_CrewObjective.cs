using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrewObjective : UI_Base
{
    
    enum Texts
    {
        Title_text,
        CollectBattery_text,
        Escape_text,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TMP_Text>(typeof(Texts));
        GetText(Texts.Escape_text).gameObject.SetActive(false);
        return true;
    }

    public void UpdateUI(int count)
    {
        GetText(Texts.CollectBattery_text).text = $"1. Collect and charge the batteries ({count}/{Define.BATTERY_COLLECT_GOAL})";
        if (count >= Define.BATTERY_COLLECT_GOAL)
        {
            OnBatteryCollectComplete();
        }
    }

    private void OnBatteryCollectComplete()
    {
        GetText(Texts.CollectBattery_text).text =
            $"1. Collect and charge the batteries ({Define.BATTERY_COLLECT_GOAL}/{Define.BATTERY_COLLECT_GOAL})";
        GetText(Texts.CollectBattery_text).fontStyle = FontStyles.Strikethrough;
        GetText(Texts.CollectBattery_text).color = Color.gray;
        GetText(Texts.Escape_text).gameObject.SetActive(true);
    }
}

