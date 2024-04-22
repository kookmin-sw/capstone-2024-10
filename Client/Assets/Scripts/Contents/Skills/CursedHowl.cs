using System.Collections;

public class CursedHowl : BaseSkill
{
    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);

        ReadySkillActionType = Define.AlienActionType.ReadyCursedHowl;
        SkillActionType = Define.AlienActionType.CursedHowl;
    }

    protected override IEnumerator ProgressSkill()
    {
        PlayAnim(false);
        PlaySound();

        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            // TODO

            UpdateWorkAmount();
            yield return null;
        }

        SkillInterrupt();
    }
}
