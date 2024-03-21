using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrewHP : UI_Base
{
    public Crew Crew { get; set; }
    private Image _fill;

    enum Images
    {
        Fill
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));
        _fill = GetImage((int)Images.Fill);
        return true;
    }

    private void Update()
    {
        if (Crew == null) return;
    
        _fill.fillAmount = (float) Crew.CrewStat.Hp / Crew.CrewStat.MaxHp;
        
    }
}
