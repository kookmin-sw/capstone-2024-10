using System.Collections;
using UnityEngine;

public class CursedHowl : BaseSkill
{
    public override void ReadySkill()
    {
        Owner.IngameUI.WorkProgressBarUI.Show(SkillData.Name, CurrentReadySkillAmount, SkillData.TotalReadySkillAmount);
        Owner.CreatureState = Define.CreatureState.Use;
        Owner.CreaturePose = Define.CreaturePose.Stand;

        Owner.AlienAnimController.PlayReadyCursedHowl();

        StartCoroutine(ReadySkillProgress());
    }

    public override void UseSkill()
    {
        base.UseSkill();

        Owner.AlienAnimController.PlayCursedHowl();

        StartCoroutine(ProgressSkill());
    }

    protected override IEnumerator ProgressSkill()
    {
        while (CurrentSkillAmount < SkillData.TotalSkillAmount)
        {
            // TODO

            UpdateWorkAmount();
            yield return null;
        }

        SkillInterrupt();
    }
}
