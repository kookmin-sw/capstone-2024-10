public class Stalker : Alien
{
    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        SkillController.Skills[0] = gameObject.GetComponent<BasicAttack>();
        SkillController.Skills[1] = gameObject.GetComponent<Roar>();
        SkillController.Skills[2] = gameObject.GetComponent<LeapAttack>();
        SkillController.Skills[3] = gameObject.GetComponent<CursedHowl>();

        SkillController.Skills[0].SetInfo(Define.SKILL_BASIC_ATTACK_ID);
        SkillController.Skills[1].SetInfo(Define.SKILL_ROAR_ID);
        SkillController.Skills[2].SetInfo(Define.SKILL_LEAP_ATTACK_ID);
        SkillController.Skills[3].SetInfo(Define.SKILL_CURSED_HOWL_ID);
    }
}
