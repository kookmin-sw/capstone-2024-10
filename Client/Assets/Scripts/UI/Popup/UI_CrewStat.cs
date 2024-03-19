using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CrewStat : UI_Popup
{
    enum Texts
    {
        StaminaCount,
        HealthCount,
    }
    enum Sliders
    {
        Stamina,
        Health,
    }

    public Crew CurrentCrew { get; set; }
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<TMP_Text>(typeof(Texts));
        Bind<Slider>(typeof(Sliders));


        GetSlider((int)Sliders.Stamina).maxValue = 100.0f;
        GetText((int)Texts.StaminaCount).text = "100";
        GetSlider((int)Sliders.Health).maxValue = 100.0f;
        GetText((int)Texts.HealthCount).text = "100";
        return true;
    }

    private void Update()
    {
        if (CurrentCrew != null)
        {
            GetSlider((int)Sliders.Stamina).value = CurrentCrew.CrewStat.Stamina;
            GetText((int)Texts.StaminaCount).text = $"{(int)CurrentCrew.CrewStat.Stamina}";
            StaminaColorChange();

            GetSlider((int)Sliders.Health).value = CurrentCrew.CrewStat.Hp;
            GetText((int)Texts.HealthCount).text = $"{(int)CurrentCrew.CrewStat.Hp}";
        }
    }

    private void StaminaColorChange()
    {
        if (CurrentCrew.CanRun)
        {
            GetSlider((int)Sliders.Stamina).fillRect.GetComponent<Image>().color = Color.green;
        }
        else
        {
            GetSlider((int)Sliders.Stamina).fillRect.GetComponent<Image>().color = Color.red;
        }

    }
}
