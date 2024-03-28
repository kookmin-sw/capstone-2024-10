using DG.Tweening;

public abstract class BaseSkill
{
    public Alien Owner { get; set; }

    public float SkillTime { get; protected set; } = 1.5f;
    public float CoolTime { get; protected set; } = 2f;
    public bool Ready { get; protected set; } = true;

    public abstract bool CheckAndUseSkill();

    public virtual void UseSkill()
    {
        Cooldown();
    }

    public void Cooldown()
    {
        Ready = false;
        DOVirtual.DelayedCall(CoolTime, () => { Ready = true; });
    }
}

