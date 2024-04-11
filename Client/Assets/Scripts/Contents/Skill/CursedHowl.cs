using System.Collections;
using UnityEngine;

public class CursedHowl : BaseSkill
{
    protected override void Init()
    {
        base.Init();

        SkillDescription = "CURSED HOWL";
        CoolTime = 4f;
        TotalSkillAmount = 2.2f;
        TotalReadySkillAmount = 1f;
        AttackRange = 0f;
    }

    public override void ReadySkill()
    {
        Owner.IngameUI.WorkProgressBarUI.Show(SkillDescription, CurrentReadySkillAmount, TotalReadySkillAmount);
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
        while (CurrentSkillAmount < TotalSkillAmount)
        {
            // TODO

            UpdateWorkAmount(Time.deltaTime);
            yield return null;
        }

        SkillInterrupt();
    }
}
