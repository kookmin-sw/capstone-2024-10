public class Stalker : Alien
{
    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        SkillController.Skills[0] = gameObject.GetComponent<BasicAttack>();
        SkillController.Skills[1] = gameObject.GetComponent<Roar>();
    }
}
