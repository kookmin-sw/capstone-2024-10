public class Stalker : Alien
{
    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        for (int i = 0; i < Define.MAX_SKILL_NUM; i++)
        {
            Skills.Add(new BasicAttack());
            Skills[i].Owner = this;
        }
    }
}
