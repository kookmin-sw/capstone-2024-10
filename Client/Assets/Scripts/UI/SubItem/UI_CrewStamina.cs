using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrewStamina : UI_Base
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
        
        _fill.fillAmount = Crew.CrewStat.Stamina / Crew.CrewStat.MaxStamina;
        _fill.color = Crew.IsRecoveringStamina ? Color.red : Color.white;
        _fill.DOFade(_fill.fillAmount > 0.9f ? 0 : 1, 0.5f);
    }
}
