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

        if (Managers.GameMng.MapSystem.Sectors[Owner.CurrentSector].IsEroded)
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

        Managers.GameMng.MapSystem.Sectors[Owner.CurrentSector].Rpc_ApplyErosion();

        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            UpdateWorkAmount();
            yield return null;
        }

        SkillInterrupt(0f);
    }
}
