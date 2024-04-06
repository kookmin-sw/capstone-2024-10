public class CursedHowl : BaseSkill
{
    protected override void Init()
    {
        base.Init();

        SkillDescription = "CURSED HOWL";
        SkillTime = 2.1f;
        CoolTime = 4f;
        TotalSkillAmount = 3f;
    }

    public override bool CheckAndUseSkill()
    {
        if (!Ready)
            return false;

        ReadySkill();
        return true;
    }

    public override void ReadySkill()
    {
        Owner.IngameUI.WorkProgressBarUI.Show(SkillDescription, CurrentSkillAmount, TotalSkillAmount);
        Owner.CreatureState = Define.CreatureState.Use;
        Owner.CreaturePose = Define.CreaturePose.Stand;

        Owner.AlienAnimController.PlayReadyCursedHowl();

        StartCoroutine(CoReadySkill());
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Owner.AlienAnimController.PlayCursedHowl();

        // TODO - 전역 효과 구현

        SkillInterrupt();
        Owner.ReturnToIdle(SkillTime);
    }
}
