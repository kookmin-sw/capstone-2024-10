public class Stalker : Alien
{
    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        Skills[0] = new BasicAttack();
    }
}
