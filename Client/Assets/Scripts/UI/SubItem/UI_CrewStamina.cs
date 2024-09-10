using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrewStamina : UI_Base
{
    public Crew Crew { get; set; }
    private Image _fill;
    private float timer = 60.0f;

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

        if (Crew.Object == null || !Crew.Object.IsValid) return;

        if (Crew.CrewStat.Doped)
        {
            _fill.fillAmount = Crew.CrewStat.Stamina / Crew.CrewStat.MaxStamina;
            _fill.color = Color.yellow;
            _fill.DOFade(_fill.fillAmount > 0.9f ? 0 : 1, 0.5f);
        }
        else
        {
            _fill.fillAmount = Crew.CrewStat.Stamina / Crew.CrewStat.MaxStamina;
            _fill.color = Crew.CrewStat.IsRunnable ? Color.white : Color.red;
            _fill.DOFade(_fill.fillAmount > 0.9f ? 0 : 1, 0.5f);
        }
    }
}
