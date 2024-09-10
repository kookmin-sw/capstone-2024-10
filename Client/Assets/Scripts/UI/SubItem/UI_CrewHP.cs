using UnityEngine;
using UnityEngine.UI;

public class UI_CrewHP : UI_Base
{
    public Crew Crew { get; set; }
    //private Image _fill;
    public Sprite emptyHeart;
    public Sprite fullHeart;
    public Image[] hearts;

    enum Images
    {
        //Fill
        Image1,
        Image2,
        Image3
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Bind<Image>(typeof(Images));
        //_fill = GetImage((int)Images.Fill);
        for (int i = 0; i < 3; i++)
        {
            hearts[i] = GetImage((int)Images.Image1 + i);
        }
        return true;
    }

    private void Update()
    {
        if (Crew == null) return;
        if (Crew.Object == null || !Crew.Object.IsValid) return;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < Crew.CrewStat.Hp)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            if (i < Crew.CrewStat.MaxHp)
            {
                hearts[i].enabled = true;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
        //_fill.fillAmount = (float) Crew.CrewStat.Hp / Crew.CrewStat.MaxHp;
    }
}
