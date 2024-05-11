using System.Collections;

public class CursedHowl : BaseSkill
{
    public override void SetInfo(int templateId)
    {
        base.SetInfo(templateId);

        ReadySkillActionType = Define.AlienActionType.ReadyCursedHowl;
        SkillActionType = Define.AlienActionType.CursedHowl;
    }

    public override bool CheckAndUseSkill()
    {
        if (CurrentCoolTime > 0f)
            return false;

        if (Managers.GameMng.MapSystem.Sectors[Owner.CurrentSector].IsErosion)
            return false;

        if (SkillData.Range > 0f)
            Owner.CurrentSkillRange = SkillData.Range;

        ReadySkill();
        return true;
    }

    protected override IEnumerator ProgressSkill()
    {
        PlayAnim(false);
        PlaySound();

        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            Managers.GameMng.MapSystem.Sectors[Owner.CurrentSector].GetErosion();

            UpdateWorkAmount();
            yield return null;
        }

        SkillInterrupt();
    }
}
