using System.Collections.Generic;
using UnityEngine.UI;

public class UI_AlienSkill : UI_Base
{
    public Alien Alien { get; set; }
    public Dictionary<int, BaseSkill> AlienSkills => Alien.SkillController.Skills;
    public Image[] imageCooldowns;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        for (int i = 0; i < 3; i++)
            imageCooldowns[i].fillAmount = 0f;

        return true;
    }

    public override void OnUpdate()
    {
        if (Alien == null) return;

        if (Alien.Object == null || !Alien.Object.IsValid) return;

        for (int i = 0; i < 3; i++)
        {
            if (AlienSkills[i + 1] is LeapAttack leapAttack)
            {
                if (!leapAttack.IsCurrentSectorEroded)
                {
                    imageCooldowns[i].fillAmount = 1f;
                    continue;
                }
            }

            imageCooldowns[i].fillAmount = AlienSkills[i + 1].CurrentCoolTime / AlienSkills[i + 1].SkillData.CoolTime;
        }
    }
}
